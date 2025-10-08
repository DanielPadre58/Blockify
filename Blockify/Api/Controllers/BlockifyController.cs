using System.Security.Claims;
using Blockify.Api.Controllers.Communication;
using Blockify.Application.DTOs;
using Blockify.Application.Exceptions;
using Blockify.Application.Services.Blockify;
using Blockify.Application.Services.Spotify;
using Blockify.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Blockify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockifyController : ControllerBase
{
    private const string BlockifyIdUrn = "urn:blockify:user_id";
    private readonly ISpotifyService _spotifyService;
    private readonly IBlockifyService _blockifyService;

    public BlockifyController(ISpotifyService spotifyService, IBlockifyService blockifyService)
    {
        _spotifyService = spotifyService;
        _blockifyService = blockifyService;
    }

    private long GetUserId()
    {
        if (!long.TryParse(User.FindFirst(BlockifyIdUrn)?.Value, out var userId))
            throw new UnauthorizedAccessException("User ID claim missing or invalid");

        return userId;
    }
        
    private string GetSpotifyId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                          ?? throw new MissingPrincipalClaimException("spotify:userId");

        return userIdClaim.Value;
    }

    [HttpGet("playlists")]
    public async Task<IActionResult> GetUsersPlaylists()
    {
        try
        {
            var userId = GetUserId();
            var result = await _spotifyService.GetUserPlaylistsAsync(userId);

            if (!result.IsSuccess())
                throw new UnsuccessfulResultException(result.GetError()!);

            return Ok(
                ResponseModel<IEnumerable<PlaylistDto>>.Ok(result.GetValue()));
        }
        catch (UnsuccessfulResultException ex)
        { return this.ErrorResponse(ex.Error.StatusCode, ex.Error); }
    }

    [HttpPost("playlists/{keyword}")]
    public async Task<IActionResult> CreateKeywordPlaylist([FromRoute] string keyword)
    {
        try
        {
            var userId = GetUserId();
            var result = await _spotifyService.CreateKeywordPlaylistAsync(userId, keyword);

            if (!result.IsSuccess())
                throw new UnsuccessfulResultException(result.GetError()!);

            return Ok(
                ResponseModel<PlaylistDto>.Ok(result.GetValue()));
        }
        catch (UnsuccessfulResultException ex)
        { return this.ErrorResponse(ex.Error.StatusCode, ex.Error); }
    }

    [HttpPost("playlists/{playlistId}/tracks")]
    public async Task<IActionResult> AddTracksToPlaylist([FromRoute] string playlistId, [FromBody] IEnumerable<string> trackUris)
    {
        try
        {
            var userId = GetUserId();
            //TODO THE SERVICE CLASS SHOULD BE THE ONE RESPONSIBLE FOR ACCESSING THE TOKEN
            var accessToken = await _spotifyService.GetAccessTokenByIdAsync(userId);
            var result = await _spotifyService.AddTracksToPlaylistAsync(playlistId, trackUris, accessToken);

            if (!result.IsSuccess())
                throw new UnsuccessfulResultException(result.GetError()!);

            return Ok(ResponseModel<object>.Ok());
        }
        catch (UnsuccessfulResultException ex)
        { return this.ErrorResponse(ex.Error.StatusCode, ex.Error); }
    }
    
    [HttpGet("playlists/keywords")]
    public async Task<IActionResult> GetAllUsersKeywords()
    {
        try
        {
            var userId = GetSpotifyId();
            var keywords = await _blockifyService.GetAllUsersKeywords(userId);

            return Ok(
                ResponseModel<IEnumerable<string>>.Ok(keywords));
        }
        catch (UnsuccessfulResultException ex)
        { return this.ErrorResponse(ex.Error.StatusCode, ex.Error); }
    }
}
