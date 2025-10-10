using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Singular;

public abstract class SpotifyOwner
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}