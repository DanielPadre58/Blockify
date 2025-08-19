using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;

namespace Blockify.Api.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
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
            var result = await HttpContext.AuthenticateAsync("spotify");

            if (result?.Succeeded == true)
            {
                var accessToken = await HttpContext.GetTokenAsync("spotify", "access_token");
                var refreshToken = await HttpContext.GetTokenAsync("spotify", "refresh_token");

                var userId = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = result.Principal?.FindFirst(ClaimTypes.Name)?.Value;

                await HttpContext.SignInAsync("default_cookie", result.Principal!, result.Properties!);

                return Ok(new ResponseModel<UserAuthenticationDto>(
                    true,
                    "User authenticated successfully!",
                    new UserAuthenticationDto(
                        userId!,
                        userName!,
                        new TokenDto(accessToken!, refreshToken!)
                    )
                ));
            }

            return Ok(new ResponseModel<TokenDto>(
                    false,
                    "Authentication failed"
                ));
        }

        [HttpGet("logout")]
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