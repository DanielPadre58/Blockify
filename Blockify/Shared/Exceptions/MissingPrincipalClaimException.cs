namespace Blockify.Shared.Exceptions
{
    public class MissingPrincipalClaimException : Exception
    {
        public MissingPrincipalClaimException()
            : base("There is a missing claim in the authentication result provided") { }

        public MissingPrincipalClaimException(string claimName, Exception? innerException)
            : base(
                $"{claimName} is missing claim in the authentication result provided",
                innerException
            ) { }
    }
}
