namespace Blockify.Infrastructure.External.Genius.Mappers;

public record GeniusResponse
{
    public List<GeniusHit> Hits { get; init; }
}