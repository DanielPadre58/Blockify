using System.Text.Json.Serialization;

namespace Blockify.Application.DTOs.Authentication;

public record SpotifyDto
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("profile_url")]
    public required string Url { get; set; }
    
    [JsonPropertyName("username")]
    public required string Username { get; set; }
    
    [JsonPropertyName("token_data")] 
    public TokenDto Token { get; set; }
}