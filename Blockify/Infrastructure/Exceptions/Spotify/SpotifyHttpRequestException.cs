namespace Blockify.Infrastructure.Exceptions.Spotify;

public class SpotifyHttpRequestException : Exception
{
    public string EndpointUri { get; }
    public string SpotifyMessage { get; }

    public SpotifyHttpRequestException(string message)
        : this(string.Empty, message, string.Empty, null) { }

    public SpotifyHttpRequestException(string? uri, string message)
        : this(uri, message, string.Empty, null) { }

    public SpotifyHttpRequestException(string? uri, string message, string spotifyMessage)
        : this(uri, message, spotifyMessage, null) { }

    public SpotifyHttpRequestException(string? uri, string message, string spotifyMessage, Exception? innerException)
        : base(message, innerException)
    {
        EndpointUri = uri ?? string.Empty;
        SpotifyMessage = spotifyMessage;
    }
}
