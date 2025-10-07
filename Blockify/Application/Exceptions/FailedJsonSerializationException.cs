namespace Blockify.Application.Exceptions;

public class FailedJsonSerializationException : Exception
{
    public string OriginalJson { get; }
    public readonly int _statusCode = StatusCodes.Status500InternalServerError;

    public FailedJsonSerializationException(string message)
        : this(message, string.Empty, null) { }

    public FailedJsonSerializationException(string message, string json)
        : this(message, json, null) { }

    public FailedJsonSerializationException(string message, string json, Exception? innerException)
        : base(message, innerException)
    {
        OriginalJson = json;
    }
}