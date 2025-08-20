using Blockify.Application.DTOs.Authentication;
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

                return new(
                        userId ?? throw new MissingPrincipalClaimException("userId"),
                        userName ?? throw new MissingPrincipalClaimException("userName"),
                        new(
                            accessToken ?? throw new MissingPrincipalClaimException("Token.AcessToken"),
                            refreshToken ?? throw new MissingPrincipalClaimException("Token.RefreshToken")
                        )
                    );
            }

            throw new AuthenticationException();
        }
    }
}