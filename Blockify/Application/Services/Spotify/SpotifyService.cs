using Blockify.Application.Services.Spotify.Client;

namespace Blockify.Application.Services.Spotify {
    public class SpotifyService : ISpotifyService
    {
        private readonly ISpotifyClient _spotifyClient;

        public SpotifyService(ISpotifyClient spotifyClient)
        {
            _spotifyClient = spotifyClient;
        }
    }
}