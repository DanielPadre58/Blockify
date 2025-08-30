using Blockify.Application.Services.Spotify;
using Microsoft.AspNetCore.Mvc;

namespace Blockify.Application.DTOs.External
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockifyController : ControllerBase
    {
        private readonly ISpotifyService _spotifyService;

        public BlockifyController(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        [HttpGet("user/playlists")]
        public async Task<IActionResult> GetUsersPlaylists() {
            try
            {
                var userId = Convert.ToInt64(User.FindFirst("urn:blockify:user_id")?.Value);
                var response = await _spotifyService.GetUsersPlaylists(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "An error occurred while fetching user playlists: " + ex.Message
                );
            }
        }

        [HttpGet("playlist/{playlistId}")]
        public async Task<IActionResult> GetPlaylist([FromRoute] string playlistId)
        {
            try
            {
                var userId = Convert.ToInt64(User.FindFirst("urn:blockify:user_id")?.Value);
                var response = await _spotifyService.GetPlaylistAsync(
                    playlistId,
                    _spotifyService.GetAccessTokenById(userId)
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "An error occurred while fetching the playlist: " + ex.Message
                );
            }
        }
    }
}
