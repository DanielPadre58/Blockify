using System.Text.Json.Serialization;

namespace Blockify.Application.Services.Spotify.Mappers.Singular;

public class SpotifyTrackObject
{
    [JsonPropertyName("track")]
    public required SpotifyTrack Track { get; init; }
}