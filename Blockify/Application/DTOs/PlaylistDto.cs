
namespace Blockify.Application.DTOs;

public record PlaylistDto
{
    public required string OwnerId { get; set; }
    public required string SpotifyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}