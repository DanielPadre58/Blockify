using Blockify.Application.DTOs.Authentication;

namespace Blockify.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public required string Email { get; set; }
    public required SpotifyDto Spotify { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastRequestDate { get; set; }
}