using System.Text.Json.Serialization;
using Blockify.Domain.Entities;

namespace Blockify.Domain.Spotify.Mappers.Singular;

public class SpotifyTracks
{
    [JsonPropertyName("items")]
    public List<SpotifyTrackObject> Items { get; set; } = [];
}