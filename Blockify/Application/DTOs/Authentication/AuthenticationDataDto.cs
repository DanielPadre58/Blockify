namespace Blockify.Application.DTOs.Authentication
{
    public class AuthenticationDataDto
    {
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; init; }
        public string? TokenType { get; init; }
        public string? Expiry { get; init; }
        public string? UserId { get; init; }
        public string? Username { get; init; }
    }
}
