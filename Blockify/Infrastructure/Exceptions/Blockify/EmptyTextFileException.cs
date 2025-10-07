namespace Blockify.Infrastructure.Exceptions.Blockify;

public class EmptyTextFileException : Exception
{
    public string? FilePath { get; }

    public EmptyTextFileException(string message)
        : this(message, null, null) { }

    public EmptyTextFileException(string message, string filePath)
        : this(message, filePath, null) { }

    public EmptyTextFileException(string message, string? filePath, Exception? innerException)
        : base(message, innerException)
    {
        FilePath = string.IsNullOrWhiteSpace(filePath) ? null : filePath;
    }
}
