namespace Blockify.Application.Exceptions;

public class ResourceNotFoundException : Exception
{
    public string Resource { get; set; }
    public readonly int _statusCode = StatusCodes.Status404NotFound;

    public ResourceNotFoundException(string message)
        : this(message, string.Empty) { }

    public ResourceNotFoundException(string message, string resource) : base(message)
    {
        Resource = resource;
    }
}
