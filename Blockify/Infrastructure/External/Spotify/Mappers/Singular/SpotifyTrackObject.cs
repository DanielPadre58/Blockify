using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Singular;

public abstract class SpotifyTrackObject
{
    [JsonPropertyName("track")]
    public required SpotifyTrack Track { get; init; }
}