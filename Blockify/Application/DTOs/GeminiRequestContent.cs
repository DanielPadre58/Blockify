namespace Blockify.Application.DTOs;

public record GeminiRequestContent
{
    public required string Objective { get; init; }
    public List<string>? ExistingKeywords { get; init; }
    public required string Name { get; init; }
    public string? Artist { get; init; }
    public string? Genre { get; init; }
    public required string Lyrics { get; init; }
}