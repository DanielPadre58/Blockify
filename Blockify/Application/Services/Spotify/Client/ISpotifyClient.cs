using Blockify.Application.DTOs.Authentication;
using Blockify.Domain.ExternalEntities.Spotify;

namespace Blockify.Application.Services.Spotify.Client
{
    public interface ISpotifyClient
    {
        public Task<TokenDto> RefreshTokenAsync(string refreshToken);

        public Task<Playlist> GetPlaylistAsync(string playlistId, string accessToken);
    }
}
