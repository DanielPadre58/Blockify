namespace Blockify.Infrastructure.External.Genius.Mappers;

public record GeniusHit
{
    public string Type { get; set; }
    public GeniusHitResult Result { get; set; }
}