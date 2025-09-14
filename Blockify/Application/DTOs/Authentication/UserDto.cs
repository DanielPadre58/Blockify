using System.Text.Json.Serialization;
using static Blockify.Domain.Entities.User;

namespace Blockify.Application.DTOs.Authentication;

public record UserDto
{
    [JsonPropertyName("user_id")]
    public long Id { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("spotify_data")]
    public required SpotifyDto Spotify { get; set; } 
}