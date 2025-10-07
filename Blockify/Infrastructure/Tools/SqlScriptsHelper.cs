using System.Reflection.Metadata;

namespace Blockify.Infrastructure.Tools;

public static class SqlScriptsHelper
{
    private static string QueriesPath => Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Blockify", "Queries");

    public static string GetPath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name must not be null or whitespace.", nameof(fileName));

        if (!fileName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("File name must have a .sql extension.", nameof(fileName));

        return Path.Combine(QueriesPath, fileName);
    } 

    public static async Task<string> ReadFileAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File path must not be null or whitespace.", nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException("File not found.", path);

        try
        {
            return await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
        }
        catch (IOException ex)
        {
            throw new IOException($"I/O error reading file '{path}'.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Access denied reading file '{path}'.", ex);
        }
    }
}
