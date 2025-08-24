using System.Text.Json;
using Blockify.Application.DTOs.Authentication;
using Microsoft.Extensions.Options;

namespace Blockify.Application.Services.Spotify.Client
{
    public class SpotifyClient : ISpotifyClient
    {
        private readonly HttpClient _httpClient;
        private readonly SpotifyConfiguration _spotifyConfiguration;

        public SpotifyClient(
            HttpClient httpClient,
            IOptions<SpotifyConfiguration> spotifyConfiguration
        )
        {
            _httpClient = httpClient;
            _spotifyConfiguration = spotifyConfiguration.Value;
        }

        public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "token");

            var requestContent = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", _spotifyConfiguration.ClientId },
                { "client_secret", _spotifyConfiguration.ClientSecret },
            };

            request.Content = new FormUrlEncodedContent(requestContent);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var content = JsonSerializer.Deserialize<TokenDto>(json);

            if (content?.RefreshToken == null)
            {
                content!.RefreshToken = refreshToken;
            }

            return content!;
        }
    }
}
