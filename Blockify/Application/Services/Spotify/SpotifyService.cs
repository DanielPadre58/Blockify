using Blockify.Application.DTOs;
using Blockify.Application.DTOs.Result;
using Blockify.Application.Exceptions;
using Blockify.Application.Services.Authentication;
using Blockify.Application.Services.Spotify.Mappers.Multiple;
using Blockify.Domain.Entities;
using Blockify.Domain.Spotify.Mappers.Singular;
using Blockify.Infrastructure.Blockify.Repositories;
using Blockify.Infrastructure.Exceptions.Blockify;
using Blockify.Infrastructure.Exceptions.Spotify;
using Blockify.Infrastructure.Spotify.Client;

namespace Blockify.Application.Services.Spotify;

public class SpotifyService : ISpotifyService
{
    private readonly ISpotifyClient _spotifyClient;
    private readonly IBlockifyRepository _blockifyRepo;
    private readonly IUserAuthenticationService _authenticationService;

    public SpotifyService(ISpotifyClient spotifyClient, IBlockifyRepository blockifyDbService, IUserAuthenticationService authenticationService)
    {
        _spotifyClient = spotifyClient;
        _blockifyRepo = blockifyDbService;
        _authenticationService = authenticationService;
    }

    public async Task<PlaylistDto> GetPlaylistAsync(string playlistId, string accessToken)
    {
        var response = await _spotifyClient.GetPlaylistAsync(playlistId, accessToken);

        var json = await response.Content.ReadAsStringAsync();

        var playlist = JsonMapper<SpotifyPlaylist>.FromJson(json).ToDto();

        return playlist;
    }

    public async Task<IResult<IEnumerable<PlaylistDto>>> GetUserPlaylistsAsync(long userId)
    {
        try
        {
            var token = await _blockifyRepo.GetTokenByIdAsync(userId)
            ?? throw new ResourceNotFoundException($"There's no Spotify token associated with the user {userId}", "Spotify.Token");

            if (token.IsAlmostExpired())
                token = await _authenticationService.RefreshTokenAsync(userId);

            var response = await _spotifyClient.GetUserPlaylists(token.AccessToken);

            var json = await response.Content.ReadAsStringAsync();

            var content = JsonMapper<UserSpotifyPlaylists>.FromJson(json);

            return new Result<IEnumerable<PlaylistDto>>(
                await Task.WhenAll(
                    content.Items.Select(async p => await GetPlaylistAsync(p.Id, token.AccessToken))));
        }
        catch (ResourceNotFoundException ex)
        { return new Result<IEnumerable<PlaylistDto>>(ex); }
        catch (InvalidCommandException ex)
        { return new Result<IEnumerable<PlaylistDto>>(ex); }
        catch (SpotifyHttpRequestException ex)
        { return new Result<IEnumerable<PlaylistDto>>(ex); }
        catch (FailedJsonSerializationException ex)
        { return new Result<IEnumerable<PlaylistDto>>(ex); }
        catch (Exception ex)
        { return new Result<IEnumerable<PlaylistDto>>(ex); }
    }

    public async Task<IResult<PlaylistDto>> CreateKeywordPlaylistAsync(long userId, string keyword)
    {
        try
        {
            var user = (await _blockifyRepo.SelectUserByIdAsync(userId))
                        ?? throw new Exception("User not found");

            if (user.Spotify.Token.IsAlmostExpired())
                user.Spotify.Token = await _authenticationService.RefreshTokenAsync(userId);

            var response = await _spotifyClient.CreateKeywordPlaylist(user.Spotify.Token.AccessToken, user.Spotify.Id, keyword);

            var json = await response.Content.ReadAsStringAsync();

            var playlist = JsonMapper<SpotifyPlaylist>.FromJson(json).ToDto();

            await _blockifyRepo.InsertPlaylistAsync(playlist);

            return new Result<PlaylistDto>(playlist);
        }
        catch (ResourceNotFoundException ex)
        { return new Result<PlaylistDto>(ex); }
        catch (InvalidCommandException ex)
        { return new Result<PlaylistDto>(ex); }
        catch (SpotifyHttpRequestException ex)
        { return new Result<PlaylistDto>(ex); }
        catch (FailedJsonSerializationException ex)
        { return new Result<PlaylistDto>(ex); }
        catch (Exception ex)
        { return new Result<PlaylistDto>(ex); }
    }

    public async Task<string> GetAccessTokenByIdAsync(long userId)
    {
        var token = await _blockifyRepo.GetTokenByIdAsync(userId);

        return token?.AccessToken ?? throw new Exception("Token not found");
    }

    public async Task<IResult<object>> AddTracksToPlaylistAsync(string playlistId, IEnumerable<string> trackUris, string accessToken)
    {
        try
        {
            await _spotifyClient.AddTracksToPlaylistAsync(playlistId, trackUris, accessToken);
            return new Result<object>();
        }
        catch (ResourceNotFoundException ex)
        { return new Result<object>(ex); }
        catch (InvalidCommandException ex)
        { return new Result<object>(ex); }
        catch (SpotifyHttpRequestException ex)
        { return new Result<object>(ex); }
        catch (FailedJsonSerializationException ex)
        { return new Result<object>(ex); }
        catch (Exception ex)
        { return new Result<object>(ex); }
    }
}

