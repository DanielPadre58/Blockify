namespace Blockify.Application.DTOs;

public record SongDto
{
    public string Name { get; init; }
    public string Artist { get; init; }
    public string Genre { get; init; }
}