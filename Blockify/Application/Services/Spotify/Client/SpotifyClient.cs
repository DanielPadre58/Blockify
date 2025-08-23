namespace Blockify.Application.Services.Spotify.Client {
    public class SpotifyClient : ISpotifyClient
    {
        private readonly HttpClient _httpClient;

        public SpotifyClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}