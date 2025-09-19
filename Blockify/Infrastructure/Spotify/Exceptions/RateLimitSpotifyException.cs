namespace Blockify.Infrastructure.Spotify.Exceptions;

public class RateLimitSpotifyException : SpotifyHttpRequestException
{
    public int RetryAfter { get; }

    public RateLimitSpotifyException()
        : this(
            string.Empty,
            -1,
            "Spotify rate limit reached. Please wait a bit until you can send another request",
            string.Empty,
            null) { }

    public RateLimitSpotifyException(int retryAfter)
        : this(
            string.Empty,
            retryAfter,
            $"Spotify rate limit reached. Please wait {retryAfter} seconds until you can send another request",
            string.Empty,
            null) { }

    public RateLimitSpotifyException(string? uri, int retryAfter)
        : this(
            uri,
            retryAfter,
            $"Spotify rate limit reached requesting {uri}. Please wait {retryAfter} seconds until you can send another request",
            string.Empty,
            null) { }

    public RateLimitSpotifyException(string? uri, int retryAfter, string spotifyMessage)
        : this(
            uri,
            retryAfter,
            $"Spotify rate limit reached requesting {uri}. Please wait {retryAfter} seconds until you can send another request",
            spotifyMessage,
            null)
    { }

    public RateLimitSpotifyException(string? uri, int retryAfter, string spotifyMessage, Exception? innerException)
        : this(
            uri,
            retryAfter,
            $"Spotify rate limit reached requesting {uri}. Please wait {retryAfter} seconds until you can send another request",
            spotifyMessage,
            innerException)
    { }

    public RateLimitSpotifyException(string? uri, int retryAfter, string message, string spotifyMessage, Exception? innerException)
        : base(uri, message, spotifyMessage, innerException)
    {
        RetryAfter = retryAfter;
    }
}
