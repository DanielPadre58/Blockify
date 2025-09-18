using System.Text.Json.Serialization;

namespace Blockify.Application.Services.Spotify.Mappers.Singular;

public class SpotifyTracks
{
    [JsonPropertyName("items")]
    public List<SpotifyTrackObject> Items { get; set; } = [];
}