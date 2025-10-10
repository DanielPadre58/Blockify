namespace Blockify.Infrastructure.External.Genius.Client;

public interface IGeniusClient
{
    public Task<HttpResponseMessage> SearchSongId(string name, string artist);
}