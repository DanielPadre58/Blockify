namespace Blockify.Domain.Entities {
    public class User
    {
        public struct SpotifyData
        {
            public string Id { get; set; }
            public string Url { get; set; }
            public string Username { get; set; }
            public string RefreshToken { get; set; }
        }

        public Guid Id { get; set; }
        public SpotifyData Spotify { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastRequestDate { get; set; }
    }
}