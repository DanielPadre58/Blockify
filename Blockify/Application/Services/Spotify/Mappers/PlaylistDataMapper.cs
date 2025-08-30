using System.Text.Json.Serialization;

namespace Blockify.Application.Services.Spotify.Mappers
{
    public class PlaylistDataMapper
    {
        public class Playlist
        {
            [JsonPropertyName("id")]
            public required string Id { get; init; }

            [JsonPropertyName("name")]
            public required string Name { get; init; }

            [JsonPropertyName("description")]
            public required string? Description { get; init; }

            [JsonPropertyName("tracks")]
            public required Tracks Tracks { get; set; }
        }

        public class Tracks
        {
            [JsonPropertyName("items")]
            public List<PlaylistTrackObject> Items { get; set; } = new();
        }

        public class PlaylistTrackObject
        {
            [JsonPropertyName("track")]
            public required Track Track { get; init; }
        }

        public class Track
        {
            [JsonPropertyName("id")]
            public required string Id { get; init; }

            [JsonPropertyName("name")]
            public required string Name { get; init; }

            [JsonPropertyName("duration_ms")]
            public required int Duration { get; init; }
        }
    }
}
