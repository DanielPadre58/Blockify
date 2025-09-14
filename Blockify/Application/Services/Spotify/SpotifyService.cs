using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using Blockify.Application.DTOs.Authentication;
using Blockify.Application.Services.Spotify.Client;
using Blockify.Domain.Database;
using static Blockify.Application.Services.Spotify.Mappers.PlaylistDataMapper;
using static Blockify.Application.Services.Spotify.Mappers.UsersPlaylistsMapper;

namespace Blockify.Application.Services.Spotify;

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
        var response = await _spotifyClient.GetPlaylistAsync(playlistId, accessToken);
        
        var json = await response.Content.ReadAsStringAsync();

        var content =
            JsonSerializer.Deserialize<Playlist>(json)
            ?? throw new Exception("Failed to deserialize Spotify playlist response.");

        return content;
    }

    public async Task<IEnumerable<Playlist>> GetUsersPlaylistsAsync(long userId)
    {
        var accessToken = await _blockifyDbService.GetAccessTokenByIdAsync(userId)
            ?? throw new Exception("Access token not found for the given user ID.");
        
        var response = await _spotifyClient.GetUserPlaylists(accessToken);
        
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

    public async Task<Playlist> CreateKeywordPlaylistAsync(long userId, string keyword)
    {
        var user = await _blockifyDbService.SelectUserByIdAsync(userId)
                   ?? throw new Exception("User not found");
        
        var response = await _spotifyClient.CreateKeywordPlaylist(user.Spotify.Token.AccessToken, user.Spotify.Id, keyword);
        
        var json = await response.Content.ReadAsStringAsync();

        var playlist = JsonSerializer.Deserialize<Playlist>(json) 
                       ?? throw new Exception("Failed to deserialize Spotify newly playlist response.");
        
        return playlist;
    }

    public async Task<TokenDto> RefreshTokenAsync(long userId)
    {
        try
        {
            var user = await _blockifyDbService.SelectUserByIdAsync(userId);
            
            var response = await _spotifyClient.RefreshTokenAsync(user!.Spotify.Token.RefreshToken);
            
            var json = await response.Content.ReadAsStringAsync();

            var token = JsonSerializer.Deserialize<TokenDto>(json)
                ?? throw new Exception("Failed to deserialize Spotify token response.");

            token.ExpiresAt = DateTime.Now.AddSeconds(3600);
            
            await _blockifyDbService.RefreshAccessTokenAsync(userId, token);

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

    public async Task<string> GetAccessTokenByIdAsync(long userId)
    {
        var accessToken = await _blockifyDbService.GetAccessTokenByIdAsync(userId)
            ?? throw new Exception("Access token not found for the given user ID.");

        return accessToken;
    }
}