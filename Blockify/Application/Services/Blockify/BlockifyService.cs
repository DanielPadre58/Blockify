using Blockify.Application.Services.Spotify;
using Blockify.Infrastructure.Gemini;

namespace Blockify.Application.Services.Blockify;

public class BlockifyService : IBlockifyService
{
    private readonly ISpotifyService _spotifyService;
    private readonly IGeminiClient _geminiClient;
    
    public BlockifyService(ISpotifyService spotifyService, IGeminiClient geminiClient)
    {
        _spotifyService = spotifyService;
        _geminiClient = geminiClient;
    }
}