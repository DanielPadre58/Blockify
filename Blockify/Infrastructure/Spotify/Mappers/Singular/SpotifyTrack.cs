using System.Text.Json.Serialization;
using Blockify.Domain.Entities;

namespace Blockify.Domain.Spotify.Mappers.Singular;

public class SpotifyTrack
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("duration_ms")]
    public required int Duration { get; init; }
}