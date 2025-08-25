using System.Security.Claims;
using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;
using static Blockify.Domain.Entities.User;

namespace Blockify.Application.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        public async Task<UserAuthenticationDto> AuthenticateUserAsync(HttpContext context)
        {
            var result =
                await context.AuthenticateAsync("spotify")
                ?? throw new Exception("Something went wrong during authentication.");

            if (!result.Succeeded == true)
                throw new AuthenticationException("Spotify authentication failed.");

            var authData = new UserAuthenticationDto()
            {
                User = new UserDto
                {
                    Id = Guid.NewGuid(),
                    Spotify = new SpotifyData()
                    {
                        Id =
                            result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? throw new MissingPrincipalClaimException("spotify:userId"),
                        Username =
                            result.Principal?.FindFirst(ClaimTypes.Name)?.Value
                            ?? throw new MissingPrincipalClaimException("spotify:username"),
                        RefreshToken =
                            result.Properties?.GetTokenValue("refresh_token")
                            ?? throw new AuthenticationException("Refresh token not found."),
                    },
                },
                Token = new TokenDto
                {
                    AccessToken =
                        result.Properties?.GetTokenValue("access_token")
                        ?? throw new AuthenticationException("Access token not found."),
                    TokenType =
                        result.Properties?.GetTokenValue("token_type")
                        ?? throw new AuthenticationException("Token type not found."),
                    Expiry = DateTimeOffset
                        .Parse(
                            result.Properties?.GetTokenValue("expires_at")
                                ?? throw new AuthenticationException("Token expiry not found.")
                        )
                        .ToUnixTimeSeconds(),
                    RefreshToken =
                        result.Properties?.GetTokenValue("refresh_token")
                        ?? throw new AuthenticationException("Refresh token not found."),
                },
            };

            User user = CreateUser(authData);

            result
                .Principal.Identities.First()
                .AddClaim(new Claim("urn:blockify:spotify_user_id", user.Id.ToString()));
            await context.SignInAsync("default_cookie", result.Principal!, result.Properties!);

            return authData;
        }

        private User CreateUser(UserAuthenticationDto authData)
        {
            return new User()
            {
                Id = authData.User!.Id,
                Spotify = authData.User.Spotify,
                CreationDate = DateTime.Now,
                LastRequestDate = DateTime.Now,
            };

            //Check if user exists in database
            //Save user to database
        }
    }
}
