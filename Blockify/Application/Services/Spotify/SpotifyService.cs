using System.Text.Json;
using Blockify.Application.DTOs;
using Blockify.Application.Exceptions;
using Blockify.Application.Services.Authentication;
using Blockify.Application.Services.Spotify.Mappers.Multiple;
using Blockify.Domain.Entities;
using Blockify.Domain.Spotify.Mappers.Singular;
using Blockify.Infrastructure.Blockify.Repositories;
using Blockify.Infrastructure.Spotify.Client;

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

    public async Task<PlaylistDto> GetPlaylistAsync(string playlistId, string accessToken)
    {
        var response = await _spotifyClient.GetPlaylistAsync(playlistId, accessToken);

        var json = await response.Content.ReadAsStringAsync();

        var playlist = JsonMapper<SpotifyPlaylist>.FromJson(json).ToDto();

        return playlist;
    }

    public async Task<IEnumerable<PlaylistDto>> GetUsersPlaylistsAsync(long userId)
    {
        var token = await _blockifyDbService.GetTokenByIdAsync(userId);

        if (token.IsAlmostExpired())
            token = await _authenticationService.RefreshTokenAsync(userId);

        var response = await _spotifyClient.GetUserPlaylists(token.AccessToken);

        var json = await response.Content.ReadAsStringAsync();

        var content = JsonMapper<UserSpotifyPlaylists>.FromJson(json);

        return await Task.WhenAll(
            content.Items.Select(p => GetPlaylistAsync(p.Id, token.AccessToken))
        );
    }

    public async Task<PlaylistDto> CreateKeywordPlaylistAsync(long userId, string keyword)
    {
        var user = (await _blockifyDbService.SelectUserByIdAsync(userId))
            ?? throw new Exception("User not found");

        if (user.Spotify.Token.IsAlmostExpired())
            user.Spotify.Token = await _authenticationService.RefreshTokenAsync(userId);

        var response = await _spotifyClient.CreateKeywordPlaylist(user.Spotify.Token.AccessToken, user.Spotify.Id, keyword);

        var json = await response.Content.ReadAsStringAsync();

        var playlist = JsonMapper<SpotifyPlaylist>.FromJson(json).ToDto();

        await _blockifyDbService.InsertPlaylistAsync(playlist);

        return playlist;
    }

    public async Task<string> GetAccessTokenByIdAsync(long userId)
    {
        var token = await _blockifyDbService.GetTokenByIdAsync(userId);

        return token.AccessToken;
    }

    public async Task AddTracksToPlaylistAsync(string playlistId, IEnumerable<string> trackUris, string accessToken)
    {
        await _spotifyClient.AddTracksToPlaylistAsync(playlistId, trackUris, accessToken);
    }
}

