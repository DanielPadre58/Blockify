namespace Blockify.Application.DTOs;

public record GeminiSongData
{
    public required string Name { get; init; }
    public string? Artist { get; init; }
    public string? Genre { get; init; }
    public required string Lyrics { get; init; }
}