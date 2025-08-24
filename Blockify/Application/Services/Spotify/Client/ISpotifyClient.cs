using Blockify.Application.DTOs.Authentication;

namespace Blockify.Application.Services.Spotify.Client
{
    public interface ISpotifyClient
    {
        public Task<TokenDto> RefreshTokenAsync(string refreshToken);
    }
}
