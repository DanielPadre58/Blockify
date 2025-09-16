using System.Text.Json.Serialization;
using Blockify.Domain.Entities;

namespace Blockify.Application.DTOs.Authentication;

public record UserDto
{
    [JsonPropertyName("user_id")]
    public long Id { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [JsonPropertyName("spotify_data")]
    public required SpotifyDto Spotify { get; set; } 
    
    public static UserDto FromEntity(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Spotify = user.Spotify
    };
}