using Blockify.Application.DTOs.Authentication;
using static Blockify.Application.Services.Spotify.Mappers.PlaylistDataMapper;

namespace Blockify.Application.Services.Spotify;

public interface ISpotifyService
{
    public Task<string> GetAccessTokenByIdAsync(long userId);
    public Task<Playlist> GetPlaylistAsync(string playlistId, string accessToken);
    public Task<IEnumerable<Playlist>> GetUsersPlaylistsAsync(long userId);
    public Task<Playlist> CreateKeywordPlaylistAsync(long userId, string keyword);
}