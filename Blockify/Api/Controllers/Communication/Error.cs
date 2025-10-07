namespace Blockify.Api.Controllers.Communication {
    public record Error
    {
        public int StatusCode { get; set; }
        public required string Code { get; init; }
        public string? Details { get; init; }
    }
}
