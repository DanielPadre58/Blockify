using System.Text.Json.Serialization;

namespace Blockify.Application.DTOs.Authentication
{
    public record RefreshTokenDto
    {
        [JsonPropertyName("refresh_token")]
        public required string RefreshToken { get; init; }
    }
}
