using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using Blockify.Application.DTOs.Authentication;
using Blockify.Application.Services.Authentication;
using Blockify.Application.Services.Spotify.Client;
using Blockify.Domain.Database;
using Microsoft.AspNetCore.Authentication;
using static Blockify.Application.Services.Spotify.Mappers.PlaylistDataMapper;
using static Blockify.Application.Services.Spotify.Mappers.UsersPlaylistsMapper;

namespace Blockify.Application.Services.Spotify;

public class SpotifyService : ISpotifyService
{
    private readonly ISpotifyClient _spotifyClient;
    private readonly IBlockifyDbService _blockifyDbService;
    private readonly IUserAuthenticationService _authenticationService;

    public SpotifyService(ISpotifyClient spotifyClient, IBlockifyDbService blockifyDbService, IUserAuthenticationService authenticationService)
    {
        _spotifyClient = spotifyClient;
        _blockifyDbService = blockifyDbService;
        _authenticationService = authenticationService;
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
        var token = await _blockifyDbService.GetTokenByIdAsync(userId);

        if(token.IsAlmostExpired())
            token = await _authenticationService.RefreshTokenAsync(userId);
    
        var response = await _spotifyClient.GetUserPlaylists(token.AccessToken);
        
        var json = await response.Content.ReadAsStringAsync();

        var content =
            JsonSerializer.Deserialize<UsersPlaylistsWrapper>(json)
            ?? throw new Exception("Failed to deserialize Spotify playlist response.");

        if (content.Items.Count == 0)
        {
            throw new Exception("User does not have any playlist saved on his spotify profile");
        }

        return await Task.WhenAll(
            content.Items.Select(p => GetPlaylistAsync(p.Id, token.AccessToken))
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

    public async Task<string> GetAccessTokenByIdAsync(long userId)
    {
        var token = await _blockifyDbService.GetTokenByIdAsync(userId);

        return token.AccessToken;
    }
}