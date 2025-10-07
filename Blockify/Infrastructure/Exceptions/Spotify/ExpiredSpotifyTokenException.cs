namespace Blockify.Infrastructure.Exceptions.Spotify;

public class ExpiredSpotifyTokenException : SpotifyHttpRequestException
{
    public ExpiredSpotifyTokenException()
        : this(
            string.Empty,
            "Spotify token has expired. Please refresh the token.",
            string.Empty,
            null) { }

    public ExpiredSpotifyTokenException(string? uri)
        : this(
            uri,
            $"Spotify token has expired when accessing {uri}. Please refresh the token.",
            string.Empty,
            null) { }

    public ExpiredSpotifyTokenException(string? uri, string spotifyMessage)
        : this(
            uri,
            $"Spotify token has expired when accessing {uri}. Please refresh the token.",
            spotifyMessage,
            null) { }

    public ExpiredSpotifyTokenException(string? uri, string spotifyMessage, Exception? innerException)
        : this(
            uri,
            $"Spotify token has expired when accessing {uri}. Please refresh the token.",
            spotifyMessage,
            innerException){ }


    public ExpiredSpotifyTokenException(string? uri, string message, string spotifyMessage, Exception? innerException)
        : base(uri, message, spotifyMessage, innerException) { }
}