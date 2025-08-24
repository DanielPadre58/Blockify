using System.Security.Authentication;
using System.Text.Json;
using Blockify.Application.DTOs.Authentication;
using Blockify.Application.Services.Spotify.Client;

namespace Blockify.Application.Services.Spotify
{
    public class SpotifyService : ISpotifyService
    {
        private readonly ISpotifyClient _spotifyClient;

        public SpotifyService(ISpotifyClient spotifyClient)
        {
            _spotifyClient = spotifyClient;
        }

        public Task<TokenDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                return _spotifyClient.RefreshTokenAsync(refreshToken);
            }
            catch (HttpRequestException ex)
            {
                throw new AuthenticationException(
                    "Spotify authentication failed during token refresh.",
                    ex
                );
            }
            catch (JsonException ex)
            {
                throw new Exception("Failed to parse Spotify token response.", ex);
            }
        }
    }
}
