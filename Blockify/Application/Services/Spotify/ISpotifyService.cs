using Blockify.Application.DTOs.Authentication;

namespace Blockify.Application.Services.Spotify
{
    public interface ISpotifyService
    {
        public Task<TokenDto> RefreshTokenAsync(string refreshToken);
    }
}
