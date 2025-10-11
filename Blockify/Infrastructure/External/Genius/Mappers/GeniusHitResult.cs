namespace Blockify.Infrastructure.External.Genius.Mappers;

public record GeniusHitResult
{
    public long Id { get; init; }
    public string SongName { get; init; }
    public string PrimaryArtistNames { get; init; }
}