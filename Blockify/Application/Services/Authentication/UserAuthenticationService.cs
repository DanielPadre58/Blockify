using System.Security.Claims;
using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Database;
using Blockify.Domain.Entities;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;

namespace Blockify.Application.Services.Authentication;

public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly IBlockifyDbService _blockifyDbService;

    public UserAuthenticationService(IBlockifyDbService blockifyDbService)
    {
        _blockifyDbService = blockifyDbService;
    }

    public async Task<UserAuthenticationDto> AuthenticateUserAsync(HttpContext context)
    {
        var result = await context.AuthenticateAsync("spotify")
            ?? throw new Exception("Something went wrong during authentication.");

        if (!result.Succeeded)
            throw new AuthenticationException("Spotify authentication failed.");

        var authData = new UserAuthenticationDto
        {
            User = new UserDto
            {
                Email =
                    result.Principal?.FindFirst(ClaimTypes.Email)?.Value
                    ?? throw new MissingPrincipalClaimException("email"),
                Spotify = new SpotifyDto
                {
                    Id =
                        result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? throw new MissingPrincipalClaimException("spotify:userId"),
                    Username =
                        result.Principal?.FindFirst(ClaimTypes.Name)?.Value
                        ?? throw new MissingPrincipalClaimException("spotify:username"),
                    Url =
                        result.Principal?.FindFirst("urn:spotify:url")?.Value
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
                            ?? throw new AuthenticationException(
                                "Token expiry information not found"
                            )
                        )
                    }
                }
            }
        };

        var user = await CreateUserAsync(authData);

        result
            .Principal.Identities.First()
            .AddClaim(new Claim("urn:blockify:user_id", user.Id.ToString()));
        await context.SignInAsync("default_cookie", result.Principal!, result.Properties!);

        return authData;
    }

    private async Task<User> CreateUserAsync(UserAuthenticationDto authData)
    {
        var existingUser = await _blockifyDbService.SelectUserBySpotifyIdAsync(
            authData.User!.Spotify.Id
        );

        if (existingUser is not null)
            return existingUser;
            
        var user = new User
        {
            Email = authData.User!.Email,
            Spotify = authData.User!.Spotify
        };

        user = await _blockifyDbService.InsertUserAsync(user);

        return user;
    }
}