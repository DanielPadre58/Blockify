using Blockify.Application.DTOs;

namespace Blockify.Application.Services.Blockify;

public interface IBlockifyService
{
    public Task<IEnumerable<string>> GetAllUsersKeywords(string userId);
    public Task<long> GetGeniusSongId(SongDto song);
    public Task<string> GetLyrics(long songId);
}