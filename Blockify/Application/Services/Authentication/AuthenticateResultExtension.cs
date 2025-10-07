using System.Security.Claims;
using Blockify.Application.DTOs.Authentication;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;

namespace Blockify.Application.Services.Authentication {
    public static class AuthenticateResultExtension {
        public static UserDto ToUserDto(this AuthenticateResult result)
        {
            var user = new UserDto
            {
                Email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value
                            ?? throw new MissingPrincipalClaimException("email"),
                Spotify = new SpotifyDto
                {
                    Id = (result.Principal?.FindFirst(ClaimTypes.NameIdentifier)
                        ?? throw new MissingPrincipalClaimException("spotify:userId")).Value,
                    Username = (result.Principal?.FindFirst(ClaimTypes.Name)
                        ?? throw new MissingPrincipalClaimException("spotify:username")).Value,
                    Url = (result.Principal?.FindFirst("urn:spotify:url")
                        ?? throw new MissingPrincipalClaimException("spotify:url")).Value,
                    Token = new TokenDto
                    {
                        RefreshToken = result.Properties?.GetTokenValue("refresh_token")
                            ?? throw new AuthenticationException("Refresh token not found."),
                        AccessToken = result.Properties?.GetTokenValue("access_token")
                            ?? throw new AuthenticationException("Access token not found."),
                        ExpiresAt = Convert.ToDateTime(
                            result.Properties?.GetTokenValue("expires_at")
                                ?? throw new AuthenticationException("Token expiry information not found"))
                    }
                }
            };

            return user;
        }
    }
}
