namespace Blockify.Application.DTOs.Authentication {
    public record UserAuthenticationDto (
        UserDto User,
        TokenDto Token
    );
}