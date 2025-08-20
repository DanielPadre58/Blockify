namespace Blockify.Shared.Exceptions {
    public class AuthenticationException : Exception {
        public AuthenticationException() : base("Unable to authenticate user"){}
    }
}