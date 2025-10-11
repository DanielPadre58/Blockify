using System.Text.Json.Serialization;
using Blockify.Application.DTOs;

namespace Blockify.Infrastructure.External.Spotify.Mappers.Singular;

public record SpotifyPlaylist
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

    public PlaylistDto ToDto() => new()
    {
        OwnerId = Owner.Id,
        SpotifyId = this.Id,
        Name = Name,
        Description = Description ?? string.Empty
    };
}