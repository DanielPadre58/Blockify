using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Singular;

public abstract class SpotifyTracks
{
    [JsonPropertyName("items")]
    public List<SpotifyTrackObject> Items { get; set; } = [];
}