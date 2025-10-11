using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Singular;

public record SpotifyOwner
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}