using System.Text.Json.Serialization;

namespace Blockify.Application.Services.Spotify.Mappers;

public class UsersPlaylistsMapper
{
    public class UsersPlaylistsWrapper
    {
        [JsonPropertyName("items")]
        public required List<SimplifiedPlaylistObject> Items { get; set; } = new();
    }

    public class SimplifiedPlaylistObject
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("tracks")]
        public required TracksWrapper Tracks { get; set; }
    }

    public class TracksWrapper
    {
        [JsonPropertyName("href")]
        public required string Href { get; set; }

        [JsonPropertyName("total")]
        public required int Total { get; set; }
    }
}