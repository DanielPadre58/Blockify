using Blockify.Application.DTOs.Authentication;
using static Blockify.Application.Services.Spotify.Mappers.PlaylistDataMapper;

namespace Blockify.Application.Services.Spotify
{
    public interface ISpotifyService
    {
        public Task<TokenDto> RefreshTokenAsync(long userId);
        public string GetAccessTokenById(long userId);
        public Task<Playlist> GetPlaylistAsync(string playlistId, string accessToken);
        public Task<IEnumerable<Playlist>> GetUsersPlaylists(long userId);
    }
}
