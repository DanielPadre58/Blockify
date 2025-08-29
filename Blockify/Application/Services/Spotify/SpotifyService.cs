using System.Security.Authentication;
using System.Text.Json;
using Blockify.Application.DTOs.Authentication;
using Blockify.Application.Services.Spotify.Client;
using Blockify.Domain.Database;
using Blockify.Domain.ExternalEntities.Spotify;

namespace Blockify.Application.Services.Spotify
{
    public class SpotifyService : ISpotifyService
    {
        private readonly ISpotifyClient _spotifyClient;
        private readonly IBlockifyDbService _blockifyDbService;

        public SpotifyService(ISpotifyClient spotifyClient, IBlockifyDbService blockifyDbService)
        {
            _spotifyClient = spotifyClient;
            _blockifyDbService = blockifyDbService;
        }

        public async Task<Playlist> GetPlaylistAsync(string playlistId, string accessToken)
        {
            return await _spotifyClient.GetPlaylistAsync(playlistId, accessToken);
        }

        public async Task<TokenDto> RefreshTokenAsync(long userId)
        {
            try
            {
                var user = _blockifyDbService.SelectUserById(userId);
                var token = await _spotifyClient.RefreshTokenAsync(user!.Spotify.RefreshToken);
                _blockifyDbService.RefreshAccessToken(userId, token);

                return token;
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

        public string GetAccessTokenById(long userId)
        {
            var accessToken =
                _blockifyDbService.GetAccessTokenById(userId)
                ?? throw new Exception("Access token not found for the given user ID.");
            return accessToken;
        }
    }
}
