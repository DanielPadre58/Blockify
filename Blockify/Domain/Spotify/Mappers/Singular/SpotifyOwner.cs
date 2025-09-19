using System.Text.Json.Serialization;
using Blockify.Domain.Entities;

namespace Blockify.Domain.Spotify.Mappers.Singular;

public class SpotifyOwner
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}