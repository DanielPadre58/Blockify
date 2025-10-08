
namespace Blockify.Application.DTOs;

public record PlaylistDto
{
    public required string OwnerId { get; init; }
    public required string SpotifyId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
}