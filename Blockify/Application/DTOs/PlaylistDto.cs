
namespace Blockify.Application.DTOs;

public record PlaylistDto
{
    public required string OwnerId { get; set; }
    public required string SpotifyId { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
}