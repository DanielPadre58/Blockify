using Blockify.Application.DTOs.Authentication;

namespace Blockify.Application.Services {
    public interface IUserAuthenticationService {
        public Task<UserAuthenticationDto> AuthenticateUserAsync(HttpContext httpContext);
    }
}