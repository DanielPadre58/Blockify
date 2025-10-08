using Blockify.Application.Services.Authentication;
using Blockify.Application.Services.Spotify;
using Blockify.Infrastructure.Blockify.Repositories;
using Blockify.Infrastructure.Gemini;

namespace Blockify.Application.Services.Blockify;

public class BlockifyService : IBlockifyService
{
    private readonly ISpotifyService _spotifyService;
    private readonly IUserAuthenticationService _authenticationService;
    private readonly IBlockifyRepository _blockifyRepository;
    private readonly IGeminiClient _geminiClient;
    
    public BlockifyService(
        ISpotifyService spotifyService, 
        IUserAuthenticationService authService, 
        IBlockifyRepository blockifyRepository,
        IGeminiClient geminiClient)
    {
        _spotifyService = spotifyService;
        _authenticationService = authService;
        _blockifyRepository = blockifyRepository;
        _geminiClient = geminiClient;
    }

    public async Task<IEnumerable<string>> GetAllUsersKeywords(string userId)
    {
        var playlists = await _blockifyRepository.SelectPlaylistsAsync(userId);
        var keywords = playlists.Select(p => p.Name);

        return keywords;
    }
}