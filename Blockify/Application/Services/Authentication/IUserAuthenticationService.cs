using Blockify.Application.DTOs.Authentication;
using Blockify.Application.DTOs.Result;

namespace Blockify.Application.Services.Authentication;

public interface IUserAuthenticationService
{
    public Task<IResult<UserDto>> AuthenticateUserAsync(HttpContext httpContext);
    public Task<TokenDto> RefreshTokenAsync(long userId);
}
