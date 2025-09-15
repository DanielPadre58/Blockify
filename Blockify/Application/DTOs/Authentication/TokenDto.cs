using System.Text.Json.Serialization;

namespace Blockify.Application.DTOs.Authentication;

public class TokenDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "Bearer";

    [JsonPropertyName("expires_at")]
    public DateTime ExpiresAt { get; set; } = DateTime.Now;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = null!;
    
    public bool IsAlmostExpired() => ExpiresAt.AddMinutes(-5) <= DateTime.Now;
}