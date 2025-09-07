using System.Text.Json.Serialization;

namespace Blockify.Application.DTOs;

public record ResponseModel<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}