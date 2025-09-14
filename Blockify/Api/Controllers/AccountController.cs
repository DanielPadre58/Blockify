using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Application.Services.Authentication;
using Blockify.Application.Services.Spotify;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blockify.Api.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly IUserAuthenticationService _authenticationService;
    private readonly ISpotifyService _spotifyService;

    public AccountController(IUserAuthenticationService service, ISpotifyService spotifyService)
    {
        _authenticationService = service;
        _spotifyService = spotifyService;
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var userId = Convert.ToInt64(User.FindFirst("urn:blockify:user_id")?.Value);
            var token = await _spotifyService.RefreshTokenAsync(userId);
            return Ok(
                new ResponseModel<TokenDto>
                {
                    Message = "Token refreshed successfully",
                    Data = token
                });
        }
        catch (AuthenticationException ex)
        {
            return BadRequest(
                new ResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message
                });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new ResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message
                });
        }
    }

    [HttpGet("spotify")]
    public IActionResult Spotify()
    {
        var prop = new AuthenticationProperties { RedirectUri = Url.Action("SignIn") };

        return Challenge(prop, "spotify");
    }

    [HttpGet("signin")]
    public async Task<IActionResult> SignIn()
    {
        Logout();

        try
        {
            var result = await _authenticationService.AuthenticateUserAsync(HttpContext);

            return Ok(
                new ResponseModel<UserAuthenticationDto>
                {
                    Message = "User authenticated with Spotify successfully",
                    Data = result
                });
        }
        catch (MissingPrincipalClaimException ex)
        {
            return BadRequest(
                new ResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message
                });
        }
        catch (AuthenticationException ex)
        {
            return Unauthorized(
                new ResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message
                });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new ResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message
                });
        }
    }

    [HttpGet("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return SignOut("default_cookie");
    }

    [HttpGet("denied")]
    public IActionResult AccessDenied()
    {
        return Unauthorized(
            new ResponseModel<object>
            {
                Success = false,
                Message = "Access denied"
            });
    }
}