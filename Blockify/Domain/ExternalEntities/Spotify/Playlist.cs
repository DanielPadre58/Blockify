using System.Text.Json.Serialization;

namespace Blockify.Domain.ExternalEntities.Spotify
{
    public class Playlist
    {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("description")]
        public required string? Description { get; init; }

        [JsonPropertyName("tracks")]
        public required Tracks Tracks { get; set; }

        public void AddTracks(ICollection<Track> tracks)
        {
            tracks.ToList().ForEach(t => Tracks.Items.Add(new PlaylistTrackObject { Track = t }));
        }

        public int TrackCount() => Tracks.Items.Count;
    }

    public class Tracks
    {
        [JsonPropertyName("items")]
        public required List<PlaylistTrackObject> Items { get; set; }
    }
}
