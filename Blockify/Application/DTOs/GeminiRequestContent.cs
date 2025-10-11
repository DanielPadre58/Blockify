namespace Blockify.Application.DTOs;

public record GeminiRequestContent
{
    public required string Objective { get; init; }
    public List<string>? ExistingKeywords { get; init; }
    public required string Name { get; init; }
    public string? Artist { get; init; }
    public string? Genre { get; init; }
    public required string Lyrics { get; init; }

    public string ToPrompt()
    {
        return $"Objective: {Objective}\n" +
               $"///Existing keywords: {string.Join(", ", ExistingKeywords ?? [])}\n" +
               $"///Name: {Name}\n" +
               $"///Artist: {Artist ?? "Unknown"}\n" +
               $"///Genre: {Genre ?? "Unknown"}\n" +
               $"///Lyrics: {Lyrics}";
    }
}