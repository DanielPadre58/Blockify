namespace Blockify.Application.DTOs.Authentication {
    public record TokenDto (
        string AccessToken,
        string RefreshToken
    );
}