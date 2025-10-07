
namespace Blockify.Infrastructure.Exceptions.Blockify;

public class InvalidCommandException : DatabaseException
{
    public string? SqlFile { get; init; }

    public InvalidCommandException(string message)
        : this(message, null, null, null) { }

    public InvalidCommandException(string message, string action)
        : this(message, action, null, null) { }

    public InvalidCommandException(string message, string action, string sqlFile)
        : this(message, action, sqlFile, null) { }

    public InvalidCommandException(string message, string? action, string? sqlFile, Exception? innerException)
        : base(message, action, innerException)
    {
        SqlFile = sqlFile;
    }
}