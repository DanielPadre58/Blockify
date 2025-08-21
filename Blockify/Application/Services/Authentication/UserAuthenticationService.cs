using AspNet.Security.OAuth.Spotify;
using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.Entities;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Blockify.Application.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        public async Task<UserAuthenticationDto> AuthenticateUserAsync(HttpContext context)
        {
            var result = await context.AuthenticateAsync("spotify");

            if (result?.Succeeded == true)
            {
                var accessToken = await context.GetTokenAsync("spotify", "access_token");
                var refreshToken = await context.GetTokenAsync("spotify", "refresh_token");
                var userId = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = result.Principal?.FindFirst(ClaimTypes.Name)?.Value;

                await context.SignInAsync("default_cookie", result.Principal!, result.Properties!);

                var user = new User()
                {
                    Spotify = new User.SpotifyData()
                    {
                        Id = userId ?? throw new MissingPrincipalClaimException("userId"),
                        Url = SpotifyAuthenticationDefaults.UserInformationEndpoint
                            .Replace("api", "open")
                            .Replace("v1", "user")
                            .Replace("me", userId),
                        Username = userName ?? throw new MissingPrincipalClaimException("userName"),
                        RefreshToken = refreshToken ?? throw new MissingPrincipalClaimException("Token.RefreshToken")
                    },
                    CreationDate = DateTime.Now,
                    LastRequestDate = DateTime.Now
                };

                return new(
                        new UserDto()
                        {
                            Id = user.Id,
                            Spotify = user.Spotify
                        },
                        new TokenDto(
                            accessToken ?? throw new MissingPrincipalClaimException("Token.AccessToken"),
                            refreshToken
                        )
                    );
            }

            throw new AuthenticationException();
        }
    }
}