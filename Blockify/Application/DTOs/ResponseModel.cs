namespace Blockify.Application.DTOs {
    public record ResponseModel<T> (
        bool Success,
        string Message,
        T Data
    ){
        public ResponseModel(bool success, string message) : this(success, message, default!) { }
    }
}