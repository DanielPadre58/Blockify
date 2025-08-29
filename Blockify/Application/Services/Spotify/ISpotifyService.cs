using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.ExternalEntities.Spotify;

namespace Blockify.Application.Services.Spotify
{
    public interface ISpotifyService
    {
        public Task<TokenDto> RefreshTokenAsync(long userId);
        public string GetAccessTokenById(long userId);

        public Task<Playlist> GetPlaylistAsync(string playlistId, string accessToken);
    }
}
