using System.Security.Claims;
using AspNet.Security.OAuth.Spotify;
using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;

namespace Blockify.Application.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        public async Task<UserAuthenticationDto> AuthenticateUserAsync(HttpContext context)
        {
            var result = await context.AuthenticateAsync("spotify");

            if (result?.Succeeded == true)
            {
                var accessToken =
                    await context.GetTokenAsync("spotify", "access_token")
                    ?? throw new MissingPrincipalClaimException("Token.AccessToken");
                var refreshToken =
                    await context.GetTokenAsync("spotify", "refresh_token")
                    ?? throw new MissingPrincipalClaimException("Token.RefreshToken");
                var userId =
                    result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new MissingPrincipalClaimException("userId");
                var userName =
                    result.Principal?.FindFirst(ClaimTypes.Name)?.Value
                    ?? throw new MissingPrincipalClaimException("userName");

                await context.SignInAsync("default_cookie", result.Principal!, result.Properties!);

                var user = new User()
                {
                    Spotify = new User.SpotifyData()
                    {
                        Id = userId,
                        Url = SpotifyAuthenticationDefaults
                            .UserInformationEndpoint.Replace("api", "open")
                            .Replace("v1", "user")
                            .Replace("me", userId),
                        Username = userName,
                        RefreshToken = refreshToken,
                    },
                    CreationDate = DateTime.Now,
                    LastRequestDate = DateTime.Now,
                };

                return new(
                    new UserDto() { Id = user.Id, Spotify = user.Spotify },
                    new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken }
                );
            }

            throw new AuthenticationException();
        }
    }
}
