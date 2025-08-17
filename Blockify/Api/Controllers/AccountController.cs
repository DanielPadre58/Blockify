using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace Blockify.Api.Controllers {
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase{
        
        [HttpGet("login")]
        public IActionResult Login()
        {
            return Ok("Login page");
        }

        [HttpGet("spotify")]
        public IActionResult Spotify()
        {   
            var prop = new AuthenticationProperties{
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
                await HttpContext.SignInAsync("default_cookie", result.Principal, result.Properties);
                return Ok("Successfully signed in with Spotify!");
            }
            
            return BadRequest("Authentication failed");
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