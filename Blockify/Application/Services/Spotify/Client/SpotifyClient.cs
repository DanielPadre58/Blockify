using System.Text;
using System.Text.Json;
using Blockify.Api.Configuration;
using Microsoft.Extensions.Options;

namespace Blockify.Application.Services.Spotify.Client;

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

    public async Task<HttpResponseMessage> GetPlaylistAsync(string playlistId, string accessToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://api.spotify.com/v1/playlists/{playlistId}"
        );

        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return response;
    }

    public async Task<HttpResponseMessage> GetUserPlaylists(string accessToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.spotify.com/v1/me/playlists"
        );

        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();
        
        return response;
    }

    public async Task<HttpResponseMessage> CreateKeywordPlaylist(string accessToken, string userId, string keyword)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api.spotify.com/v1/users/{userId}/playlists");

        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        
        var requestContent = new Dictionary<string, string>
        {
            { "name", keyword },
            { "description", $"Playlist created by Blockify for songs with {keyword} theme" }
        };
        
        var jsonBody = JsonSerializer.Serialize(requestContent);

        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request);
        
        response.EnsureSuccessStatusCode();

        return response;
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