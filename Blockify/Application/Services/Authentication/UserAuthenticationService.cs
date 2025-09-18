using System.Security.Claims;
using System.Text.Json;
using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;
using Blockify.Infrastructure.Blockify.Repositories;
using Blockify.Infrastructure.Spotify.Client;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;

namespace Blockify.Application.Services.Authentication;

public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly IBlockifyDbService _blockifyDbService;
    private readonly ISpotifyClient _spotifyClient;

    public UserAuthenticationService(IBlockifyDbService blockifyDbService, ISpotifyClient spotifyClient)
    {
        _blockifyDbService = blockifyDbService;
        _spotifyClient = spotifyClient;
    }

    public async Task<UserDto> AuthenticateUserAsync(HttpContext context)
    {
        var result = await context.AuthenticateAsync("spotify")
            ?? throw new Exception("Something went wrong during authentication.");

        if (!result.Succeeded)
            throw new AuthenticationException("Spotify authentication failed.");

        var authData = new UserDto
        {
            Email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value
                        ?? throw new MissingPrincipalClaimException("email"),
            Spotify = new SpotifyDto
            {
                Id = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? throw new MissingPrincipalClaimException("spotify:userId"),
                Username = result.Principal?.FindFirst(ClaimTypes.Name)?.Value
                            ?? throw new MissingPrincipalClaimException("spotify:username"),
                Url = result.Principal?.FindFirst("urn:spotify:url")?.Value
                        ?? throw new MissingPrincipalClaimException("spotify:url"),
                Token = new TokenDto
                {
                    RefreshToken =
                            result.Properties?.GetTokenValue("refresh_token")
                                ?? throw new AuthenticationException("Refresh token not found."),
                    AccessToken =
                            result.Properties?.GetTokenValue("access_token")
                                ?? throw new AuthenticationException("Access token not found."),
                    ExpiresAt = Convert.ToDateTime(
                            result.Properties?.GetTokenValue("expires_at")
                                ?? throw new AuthenticationException("Token expiry information not found")
                        )
                }
            }
        };

        authData.Spotify.Token.ExpiresIn = Convert.ToInt32(
            (authData.Spotify.Token.ExpiresAt - DateTime.Now).TotalSeconds);

        var user = await CreateUserAsync(authData);

        result
            .Principal.Identities.First()
            .AddClaim(new Claim("urn:blockify:user_id", user.Id.ToString()));

        await context.SignInAsync("default_cookie", result.Principal!, result.Properties!);

        return user;
    }

    private async Task<UserDto> CreateUserAsync(UserDto authData)
    {
        var existingUser = await _blockifyDbService.SelectUserBySpotifyIdAsync(
            authData.Spotify.Id
        );

        if (existingUser is not null)
        {
            if(existingUser.Spotify.Token.IsAlmostExpired())
                await RefreshTokenAsync(existingUser.Id);

            return existingUser;
        }
            
        var user = new User
        {
            Email = authData.Email,
            Spotify = authData.Spotify
        };

        return await _blockifyDbService.InsertUserAsync(user)
            ?? throw new Exception("Failed to create user.");
    }

    public async Task<TokenDto> RefreshTokenAsync(long userId)
    {
        try
        {
            var user = await _blockifyDbService.SelectUserByIdAsync(userId);

            var response = await _spotifyClient.RefreshTokenAsync(user!.Spotify.Token.RefreshToken);

            var json = await response.Content.ReadAsStringAsync();

            var token = JsonSerializer.Deserialize<TokenDto>(json)
                ?? throw new Exception("Failed to deserialize Spotify token response.");

            token.ExpiresAt = DateTime.Now.AddSeconds(token.ExpiresIn);

            token.RefreshToken ??= user.Spotify.Token.RefreshToken;

            await _blockifyDbService.RefreshAccessTokenAsync(userId, token);

            return token;
        }
        catch (HttpRequestException ex)
        {
            throw new AuthenticationException(
                "Spotify authentication failed during token refresh.", ex);
        }
        catch (JsonException ex)
        {
            throw new Exception("Failed to parse Spotify token response.", ex);
        }
    }
}