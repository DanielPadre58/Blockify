using System.Text.Json.Serialization;
using Blockify.Domain.Entities;

namespace Blockify.Domain.Spotify.Mappers.Singular;

public class SpotifyTrackObject
{
    [JsonPropertyName("track")]
    public required SpotifyTrack Track { get; init; }
}