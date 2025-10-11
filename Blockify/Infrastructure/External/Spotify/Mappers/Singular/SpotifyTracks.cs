using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Singular;

public record SpotifyTracks
{
    [JsonPropertyName("items")]
    public List<SpotifyTrackObject> Items { get; set; } = [];
}