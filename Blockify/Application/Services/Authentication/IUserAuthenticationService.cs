using Blockify.Application.DTOs.Authentication;

namespace Blockify.Application.Services.Authentication;

public interface IUserAuthenticationService
{
    public Task<UserAuthenticationDto> AuthenticateUserAsync(HttpContext httpContext);
}