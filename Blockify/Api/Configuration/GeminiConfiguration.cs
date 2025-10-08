namespace Blockify.Api.Configuration;

public record GeminiConfiguration
{
    public required string Model { get; init; }
    public required string ApiKey { get; init; }
}