using Blockify.Application.DTOs;
using Blockify.Application.Exceptions;
using Blockify.Application.Services.Spotify;
using Microsoft.AspNetCore.Mvc;

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
            var response = await _spotifyService.GetUsersPlaylistsAsync(userId);
            return Ok(
                new ResponseModel<IEnumerable<PlaylistDto>>
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
                await _spotifyService.GetAccessTokenByIdAsync(userId)
            );
            return Ok(
                new ResponseModel<PlaylistDto>
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

    [HttpPost("playlist/{keyword}")]
    public async Task<IActionResult> CreateKeywordPlaylist([FromRoute] string keyword)
    {
        try
        {
            var userId = Convert.ToInt64(User.FindFirst("urn:blockify:user_id")?.Value);
            var response = await _spotifyService.CreateKeywordPlaylistAsync(userId, keyword);

            return Ok(
                new ResponseModel<PlaylistDto>
                {
                    Message = $"Playlist with keyword {keyword} successfully created",
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
                    Message = "Something went wrong trying to create the keyword playlist " + ex.Message
                });
        }
    }

    [HttpPost("playlist/{playlistId}/tracks")]
    public async Task<IActionResult> AddTracksToPlaylist([FromRoute] string playlistId, [FromBody] IEnumerable<string> trackUris)
    {
        try
        {
            var userId = Convert.ToInt64(User.FindFirst("urn:blockify:user_id")?.Value);
            var accessToken = await _spotifyService.GetAccessTokenByIdAsync(userId);
            await _spotifyService.AddTracksToPlaylistAsync(playlistId, trackUris, accessToken);

            return Ok(
                new ResponseModel<object>
                {
                    Message = $"Tracks successfully added to playlist",
                    Data = null
                });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new ResponseModel<object>
                {
                    Success = false,
                    Message = "Something went wrong trying to add tracks to the playlist " + ex.Message
                });
        }
    }
}