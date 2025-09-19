namespace Blockify.Infrastructure.Spotify.Exceptions;

public class ForbiddenSpotifyRequestException : SpotifyHttpRequestException
{
    public ForbiddenSpotifyRequestException() 
        : this(
            string.Empty,
            "Spotify refused the request. Please check the token's scopes or try accessing information public to you",
            string.Empty,
            null) { }

    public ForbiddenSpotifyRequestException(string? uri)
        : this(
            uri,
            $"Spotify refused the request to {uri}. Please check the token's scopes or try accessing information public to you",
            string.Empty,
            null) { }

    public ForbiddenSpotifyRequestException(string? uri, string spotifyMessage)
        : this(
            uri,
            $"Spotify refused the request to {uri}. Please check the token's scopes or try accessing information available to you",
            spotifyMessage,
            null) { }

    public ForbiddenSpotifyRequestException(string? uri, string spotifyMessage, Exception? innerException)
        : this(
            uri,
            $"Spotify refused the request to {uri}. Please check the token's scopes or try accessing information available to you",
            spotifyMessage,
            innerException) { }

    public ForbiddenSpotifyRequestException(string? uri, string message, string spotifyMessage, Exception? innerException)
        : base(
            uri,
            message,
            spotifyMessage,
            innerException) { }
}