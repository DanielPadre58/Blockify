using Blockify.Api.Controllers.Communication;

namespace Blockify.Application.Exceptions;

public class UnsuccessfulResultException : Exception
{
    public Error Error { get; set; }

    public UnsuccessfulResultException(Error error)
    {
        Error = error;
    }
}
