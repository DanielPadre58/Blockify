using System.Text.Json.Serialization;

namespace Blockify.Application.Services.Spotify.Mappers.Singular;

public class SpotifyPlaylist
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("owner")]
    public required SpotifyOwner Owner { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public required string? Description { get; init; }

    [JsonPropertyName("tracks")]
    public required SpotifyTracks Tracks { get; set; }
}