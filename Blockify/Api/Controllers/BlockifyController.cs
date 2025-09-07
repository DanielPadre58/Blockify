using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Authentication;
using Blockify.Application.Services.Spotify;
using Microsoft.AspNetCore.Mvc;
using static Blockify.Application.Services.Spotify.Mappers.PlaylistDataMapper;

namespace Blockify.Api.Controllers;

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
    public async Task<IActionResult> GetUsersPlaylists()
    {
        try
        {
            var userId = Convert.ToInt64(User.FindFirst("urn:blockify:user_id")?.Value);
            var response = await _spotifyService.GetUsersPlaylists(userId);
            return Ok(
                new ResponseModel<IEnumerable<Playlist>>
                {
                    Message = "User playlists retrieved successfully",
                    Data = response
                });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new ResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while fetching user playlists: " + ex.Message
                }
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
            return Ok(
                new ResponseModel<Playlist>
                {
                    Message = "Playlist retrieved successfully",
                    Data = response
                });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new ResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while fetching the playlist: " + ex.Message
                });
        }
    }
}