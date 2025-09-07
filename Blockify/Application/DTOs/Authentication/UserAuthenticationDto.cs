using System.Text.Json.Serialization;

namespace Blockify.Application.DTOs.Authentication;

public record UserAuthenticationDto
{
    [JsonPropertyName("user")]
    public UserDto? User { get; init; }
}