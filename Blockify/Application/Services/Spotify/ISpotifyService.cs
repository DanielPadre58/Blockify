using Blockify.Application.DTOs;

namespace Blockify.Application.Services.Spotify;

public interface ISpotifyService
{
    public Task<string> GetAccessTokenByIdAsync(long userId);
    public Task<PlaylistDto> GetPlaylistAsync(string playlistId, string accessToken);
    public Task<IEnumerable<PlaylistDto>> GetUsersPlaylistsAsync(long userId);
    public Task<PlaylistDto> CreateKeywordPlaylistAsync(long userId, string keyword);
    public Task AddTracksToPlaylistAsync(string playlistId, IEnumerable<string> trackUris, string accessToken);
}