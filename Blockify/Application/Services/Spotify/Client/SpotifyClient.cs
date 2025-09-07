using System.Text.Json;
using Blockify.Api.Configuration;
using Blockify.Application.DTOs.Authentication;
using Microsoft.Extensions.Options;
using static Blockify.Application.Services.Spotify.Mappers.PlaylistDataMapper;
using static Blockify.Application.Services.Spotify.Mappers.UsersPlaylistsMapper;

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

    public async Task<Playlist> GetPlaylistAsync(string playlistId, string accessToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://api.spotify.com/v1/playlists/{playlistId}"
        );

        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var content =
            JsonSerializer.Deserialize<Playlist>(json)
            ?? throw new Exception("Failed to deserialize Spotify playlist response.");

        return content;
    }

    public async Task<IEnumerable<Playlist>> GetUserPlaylists(string accessToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://api.spotify.com/v1/me/playlists"
        );

        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var content =
            JsonSerializer.Deserialize<UsersPlaylistsWrapper>(json)
            ?? throw new Exception("Failed to deserialize Spotify playlist response.");

        if (content.Items.Count == 0)
        {
            throw new Exception("User does not have any playlist saved on his spotify profile");
        }

        return await Task.WhenAll(
            content.Items.Select(p => GetPlaylistAsync(p.Id, accessToken))
        );
    }

    public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
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

        var json = await response.Content.ReadAsStringAsync();

        var content =
            JsonSerializer.Deserialize<TokenDto>(json)
            ?? throw new Exception("Failed to deserialize Spotify token response.");

        content.ExpiresAt = DateTime.Now.AddSeconds(3600);

        if (content?.RefreshToken == null)
        {
            content!.RefreshToken = refreshToken;
        }

        return content;
    }
}