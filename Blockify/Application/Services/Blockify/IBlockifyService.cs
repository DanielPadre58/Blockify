namespace Blockify.Application.Services.Blockify;

public interface IBlockifyService
{
    public Task<IEnumerable<string>> GetAllUsersKeywords(string userId);
}