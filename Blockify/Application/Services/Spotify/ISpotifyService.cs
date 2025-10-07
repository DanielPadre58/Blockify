using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Result;

namespace Blockify.Application.Services.Spotify;

public interface ISpotifyService
{
    public Task<string> GetAccessTokenByIdAsync(long userId);
    public Task<IResult<IEnumerable<PlaylistDto>>> GetUserPlaylistsAsync(long userId);
    public Task<IResult<PlaylistDto>> CreateKeywordPlaylistAsync(long userId, string keyword);
    public Task<IResult<object>> AddTracksToPlaylistAsync(string playlistId, IEnumerable<string> trackUris, string accessToken);
}
