using Blockify.Domain.Entities;

namespace Blockify.Application.DTOs {
    public record UserDto
    {
        public Guid Id { get; set; }
        public User.SpotifyData Spotify { get; set; }
    }
}