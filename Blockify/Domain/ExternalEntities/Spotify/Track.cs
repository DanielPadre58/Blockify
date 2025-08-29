using System.Text.Json.Serialization;

namespace Blockify.Domain.ExternalEntities.Spotify
{
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
