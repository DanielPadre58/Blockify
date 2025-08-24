namespace Blockify.Shared.Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException()
            : base("Unable to authenticate user") { }

        public AuthenticationException(string message, Exception? innerException)
            : base(message, innerException) { }
    }
}
