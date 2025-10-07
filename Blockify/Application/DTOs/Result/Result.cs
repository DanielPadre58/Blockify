using Blockify.Api.Controllers.Communication;

namespace Blockify.Application.DTOs.Result {
    public class Result<T> : IResult<T>
    {
        private bool Success { get; set; }
        private T? Value { get; set; }
        private Error? Error { get; set; }

        public Result()
        {
            Success = true;
        }

        public Result(T value)
        {
            Success = true;
            Value = value;
        }

        public Result(Exception ex)
        {
            Success = false;
            Error = new Error
            {
                Code = ex.GetType().Name,
                Details = ex.Message
            };
        }

        public bool IsSuccess()
        {
            return Success;
        }

        public Error? GetError()
        {
            return Error;
        }

        public T? GetValue()
        {
            return Value;
        }
    }
}
