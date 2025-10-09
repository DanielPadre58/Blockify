namespace Blockify.Api.Configuration.External_Services_Configuration;

public record GeminiConfiguration
{
    public required string Model { get; init; }
    public required string ApiKey { get; init; }
}