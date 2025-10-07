
namespace Blockify.Infrastructure.Exceptions.Blockify;

public class InvalidColumnOperationException : DatabaseException
{
    public InvalidColumnOperationException(string message)
        : this(message, null, null) { }

    public InvalidColumnOperationException(string message, string? action)
        : this(message, action, null) { }

    public InvalidColumnOperationException(string message, string? action, Exception? innerException)
        : base(message, action, innerException) { }
}