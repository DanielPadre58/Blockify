using Blockify.Application.DTOs;

namespace Blockify.Application.Services.Spotify.Mappers.Singular {
    public class SingularSpotifyMapper {
        public static PlaylistDto ToDto(SpotifyPlaylist playlist) => new()
        {
            OwnerId = playlist.Owner.Id,
            SpotifyId = playlist.Id,
            Name = playlist.Name,
            Description = playlist.Description ?? string.Empty
        };

    }
}