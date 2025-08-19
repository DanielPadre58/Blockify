namespace Blockify.Application.DTOs.Authentication {
    public record UserAuthenticationDto (
        string UserId,
        string Username,
        TokenDto Token
    );
}