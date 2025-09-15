using Blockify.Application.DTOs.Authentication;

namespace Blockify.Application.Services.Authentication;

public interface IUserAuthenticationService
{
    public Task<UserDto> AuthenticateUserAsync(HttpContext httpContext);
    public Task<TokenDto> RefreshTokenAsync(long userId);
}