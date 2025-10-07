using System.Text.Json.Serialization;
using Blockify.Api.Controllers.Communication;

namespace Blockify.Api.Controllers.Communication;

public record ResponseModel<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("error")]
    public Error? Error { get; set; }

    public static ResponseModel<T> Ok() =>
        new()
        {
            Success = true
        };

    public static ResponseModel<T> Ok(T? data) =>
        new()
        {
            Success = true,
            Data = data
        };

    public static ResponseModel<T> Fail(Error? error) =>
        new()
        {
            Success = false,
            Error = error
        };
}
