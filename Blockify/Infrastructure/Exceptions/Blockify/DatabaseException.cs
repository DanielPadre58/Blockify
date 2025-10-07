
namespace Blockify.Infrastructure.Exceptions.Blockify;

public class DatabaseException : Exception
{
    public string? Action { get; init; }

    public DatabaseException(string message)
        : this(message, null, null) { }

    public DatabaseException(string message, string column)
        : this(message, column, null) { }

    public DatabaseException(string message, string? action, Exception? innerException)
        : base(message, innerException)
    {
        Action = action;
    }
}