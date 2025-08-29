using static Blockify.Domain.Entities.User;

namespace Blockify.Application.DTOs
{
    public record UserDto
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public SpotifyData Spotify { get; set; }
    }
}
