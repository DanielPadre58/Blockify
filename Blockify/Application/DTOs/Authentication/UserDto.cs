using Blockify.Domain.Entities;
using static Blockify.Domain.Entities.User;

namespace Blockify.Application.DTOs
{
    public record UserDto
    {
        public required Guid Id { get; set; }
        public required SpotifyData Spotify { get; set; }
    }
}
