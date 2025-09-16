using System.Text.Json.Serialization;

namespace Blockify.Application.Services.Spotify.Mappers.Singular;

public class SpotifyOwner
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}