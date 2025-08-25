namespace Blockify.Application.DTOs.Authentication
{
    public record UserAuthenticationDto()
    {
        public UserDto? User { get; init; }
        public TokenDto? Token { get; init; }
    };
}
