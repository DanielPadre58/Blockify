using System.Security.Claims;
using System.Text.Json;
using Blockify.Application.DTOs.Authentication;
using Blockify.Application.DTOs.Result;
using Blockify.Domain.Entities;
using Blockify.Infrastructure.Blockify.Repositories;
using Blockify.Infrastructure.Exceptions.Blockify;
using Blockify.Infrastructure.External.Spotify.Client;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;

namespace Blockify.Application.Services.Authentication;

public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly IBlockifyRepository _blockifyDbService;
    private readonly ISpotifyClient _spotifyClient;

    public UserAuthenticationService(IBlockifyRepository blockifyDbService, ISpotifyClient spotifyClient)
    {
        _blockifyDbService = blockifyDbService;
        _spotifyClient = spotifyClient;
    }

    public async Task<IResult<UserDto>> AuthenticateUserAsync(HttpContext context)
    {
        try
        {
            var result = await context.AuthenticateAsync("spotify")
                ?? throw new Exception("Something went wrong during authentication.");

            if (!result.Succeeded)
                throw new AuthenticationException("Spotify authentication failed.");

            var authData = result.ToUserDto();

            authData.Spotify.Token.ExpiresIn = Convert.ToInt32(
                (authData.Spotify.Token.ExpiresAt - DateTime.Now).TotalSeconds);

            var user = await CreateUserAsync(authData);

            result
                .Principal.Identities.First()
                .AddClaim(new Claim("urn:blockify:user_id", user.Id.ToString()));

            await context.SignInAsync("default_cookie", result.Principal!, result.Properties!);

            return new Result<UserDto>(user);
        }
        catch (MissingPrincipalClaimException ex)
        { return new Result<UserDto>(ex); }
        catch (AuthenticationException ex)
        { return new Result<UserDto>(ex); }
        catch (InvalidCommandException ex)
        { return new Result<UserDto>(ex); }
        catch (Exception ex)
        { return new Result<UserDto>(ex); }
    }

    private async Task<UserDto> CreateUserAsync(UserDto authData)
    {
        var existingUser = await _blockifyDbService.SelectUserBySpotifyIdAsync(
            authData.Spotify.Id
        );

        if (existingUser is not null)
        {
            if (existingUser.Spotify.Token.IsAlmostExpired())
                existingUser.Spotify.Token = await RefreshTokenAsync(existingUser.Id);

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

            var token = JsonMapper<TokenDto>.FromJson(json);

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
