using Blockify.Application.DTOs;
using Blockify.Application.Services.Authentication;
using Blockify.Application.Services.Spotify;
using Blockify.Domain.Entities;
using Blockify.Infrastructure.Blockify.Repositories;
using Blockify.Infrastructure.External.Gemini.Client;
using Blockify.Infrastructure.External.Genius.Client;
using Blockify.Infrastructure.External.Genius.Mappers;

namespace Blockify.Application.Services.Blockify;

public class BlockifyService : IBlockifyService
{
    private readonly ISpotifyService _spotifyService;
    private readonly IUserAuthenticationService _authenticationService;
    private readonly IBlockifyRepository _blockifyRepository;
    private readonly IGeminiClient _geminiClient;
    private readonly IGeniusClient _geniusClient;
    
    public BlockifyService(
        ISpotifyService spotifyService, 
        IUserAuthenticationService authService, 
        IBlockifyRepository blockifyRepository,
        IGeminiClient geminiClient,
        IGeniusClient geniusClient)
    {
        _spotifyService = spotifyService;
        _authenticationService = authService;
        _blockifyRepository = blockifyRepository;
        _geminiClient = geminiClient;
        _geniusClient = geniusClient;
    }

    public async Task<IEnumerable<string>> GetAllUsersKeywords(string userId)
    {
        var playlists = await _blockifyRepository.SelectPlaylistsAsync(userId);
        var keywords = playlists.Select(p => p.Name);

        return keywords;
    }

    public async Task<long> GetGeniusSongId(SongDto song)
    {
        var response = await _geniusClient.SearchSongId(song.Name, song.Artist);

        var json = await response.Content.ReadAsStringAsync();

        var results = JsonMapper<GeniusResponseModel>.FromJson(json)
            .Response
            .Hits
            .Select(h => h.Result)
            .ToList();
        
        return results
            .First(h => h.SongName == song.Name && h.PrimaryArtistNames == song.Artist)
            .Id;
    }

    public Task<string> GetLyrics(long songId)
    {
        throw new NotImplementedException();
    }
}