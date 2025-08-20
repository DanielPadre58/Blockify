using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Blockify.Shared.Exceptions;

namespace Blockify.Api.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IUserAuthenticationService _authenticationService;

        public AccountController(IUserAuthenticationService service)
        {
            _authenticationService = service;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return Ok("Login page");
        }

        [HttpGet("spotify")]
        public IActionResult Spotify()
        {
            var prop = new AuthenticationProperties
            {
                RedirectUri = Url.Action("SignIn")
            };

            return Challenge(prop, "spotify");
        }

        [HttpGet("signin")]
        public async Task<IActionResult> SignIn()
        {
            try
            {
                var result = await _authenticationService.AuthenticateUserAsync(HttpContext);

                return Ok(new ResponseModel<UserAuthenticationDto>(
                    true,
                    "User authenticated with Spotify successfully",
                    result
                ));
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