using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Singular;

public abstract class SpotifyTrack
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("duration_ms")]
    public required int Duration { get; init; }
}