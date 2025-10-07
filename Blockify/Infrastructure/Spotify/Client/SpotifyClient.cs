using System.Text;
using System.Text.Json;
using Blockify.Api.Configuration;
using Blockify.Infrastructure.Exceptions.Spotify;
using Microsoft.Extensions.Options;
using Blockify.Infrastructure.Tools.Extensions;

namespace Blockify.Infrastructure.Spotify.Client;

public class SpotifyClient : ISpotifyClient
{
    private readonly HttpClient _httpClient;
    private readonly SpotifyConfiguration _spotifyConfiguration;

    public SpotifyClient(HttpClient httpClient, IOptions<SpotifyConfiguration> spotifyConfiguration
    )
    {
        _httpClient = httpClient;
        _spotifyConfiguration = spotifyConfiguration.Value;
    }

    private static async Task VerifyResponseAsync(HttpResponseMessage response, HttpRequestMessage request)
    {
        if (!response.IsSuccessStatusCode)
        {
            var spotifyMessage = await response.GetErrorMessageAsync() ?? "No error message found on Spotify API response";
            var statusCode = (int)response.StatusCode;
            var uri = request.RequestUri?.ToString();

            throw statusCode switch
            {
                401 => new ExpiredSpotifyTokenException(uri, spotifyMessage),
                403 => new ForbiddenSpotifyRequestException(uri, spotifyMessage),
                429 => new RateLimitSpotifyException(
                                        uri,
                                        response.GetRetryAfterSeconds() ?? -1,
                                        spotifyMessage),
                _ => new SpotifyHttpRequestException(
                                        uri,
                                        $"Spotify request to {uri} failed with status code {statusCode}",
                                        spotifyMessage),
            };
        }
    }

    public async Task<HttpResponseMessage> GetPlaylistAsync(string playlistId, string accessToken)
    {
        var requestUri = $"https://api.spotify.com/v1/playlists/{playlistId}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        var response = await _httpClient.SendAsync(request);
        await VerifyResponseAsync(response, request);

        return response;
    }

    public async Task<HttpResponseMessage> GetUserPlaylists(string accessToken)
    {
        var requestUri = "https://api.spotify.com/v1/me/playlists";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        
        var response = await _httpClient.SendAsync(request);
        await VerifyResponseAsync(response, request);

        return response;
    }

    public async Task<HttpResponseMessage> CreateKeywordPlaylist(string accessToken, string userId, string keyword)
    {
        var requestUri = $"https://api.spotify.com/v1/users/{userId}/playlists";
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        var requestContent = new Dictionary<string, string>
        {
            { "name", keyword },
            { "description", $"Playlist created by Blockify for songs with {keyword} theme" }
        };

        var jsonBody = JsonSerializer.Serialize(requestContent);

        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        await VerifyResponseAsync(response, request);

        return response;
    }

    public async Task AddTracksToPlaylistAsync(string playlistId, IEnumerable<string> trackUris, string accessToken)
    {
        var requestUri = $"https://api.spotify.com/v1/playlists/{playlistId}/tracks";
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        var requestContent = new Dictionary<string, IEnumerable<string>>
        {
            { "uris", trackUris }
        };

        var jsonBody = JsonSerializer.Serialize(requestContent);

        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        await VerifyResponseAsync(response, request);
    }

    public async Task<HttpResponseMessage> RefreshTokenAsync(string refreshToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://accounts.spotify.com/api/token"
        );

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

        return response;
    }
}
