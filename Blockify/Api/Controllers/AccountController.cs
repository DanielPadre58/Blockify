using Blockify.Api.Controllers.Communication;
using Blockify.Application.DTOs.Authentication;
using Blockify.Application.Exceptions;
using Blockify.Application.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blockify.Api.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly IUserAuthenticationService _authenticationService;

    public AccountController(IUserAuthenticationService service)
    {
        _authenticationService = service;
    }

    [HttpGet("spotify")]
    public async Task<IActionResult> Spotify()
    {
        await HttpContext.SignOutAsync("default_cookie");

        var properties = new AuthenticationProperties { RedirectUri = Url.Action("SignIn") };

        return Challenge(properties, "spotify");
    }

    [HttpGet("signin")]
    public async Task<IActionResult> SignIn()
    {
        try
        {
            var result = await _authenticationService.AuthenticateUserAsync(HttpContext);

            if (!result.IsSuccess())
                throw new UnsuccessfulResultException(result.GetError()!);

            return Ok(ResponseModel<UserDto>.Ok(result.GetValue()));
        }
        catch (UnsuccessfulResultException ex)
        { return this.ErrorResponse(ex.Error.StatusCode, ex.Error); }
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("default_cookie");
        return Ok(ResponseModel<object>.Ok("Logged out"));
    }

    [HttpGet("denied")]
    public IActionResult AccessDenied()
    {
        return Unauthorized(
            ResponseModel<object>.Fail(
                new Error()
                {
                    Code = "ACCESS_DENIED",
                    Details = "You were denied of accessing this resource"
                }
        ));
    }
}
