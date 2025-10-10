using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Singular;

public record SpotifyTrack
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("duration_ms")]
    public required int Duration { get; init; }
}