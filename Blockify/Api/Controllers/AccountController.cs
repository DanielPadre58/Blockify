using System.Security.Claims;
using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Application.Services;
using Blockify.Application.Services.Spotify;
using Blockify.Application.Services.Spotify.Client;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blockify.Api.Controllers
{
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
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var token = await _spotifyService.RefreshTokenAsync(refreshToken);
                return Ok(new ResponseModel<TokenDto>(true, "Token refreshed successfully", token));
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(new ResponseModel<TokenDto>(false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TokenDto>(false, ex.Message));
            }
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return Ok("Login page");
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

                var claims = User.Claims.ToList();

                return Ok(
                    new ResponseModel<UserAuthenticationDto>(
                        true,
                        "User authenticated with Spotify successfully",
                        result
                    )
                );
            }
            catch (MissingPrincipalClaimException ex)
            {
                return BadRequest(new ResponseModel<UserAuthenticationDto>(false, ex.Message));
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new ResponseModel<UserAuthenticationDto>(false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<UserAuthenticationDto>(false, ex.Message));
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
            return Unauthorized("Access denied");
        }
    }
}
