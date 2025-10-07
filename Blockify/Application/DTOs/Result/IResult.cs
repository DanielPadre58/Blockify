using Blockify.Api.Controllers.Communication;

namespace Blockify.Application.DTOs.Result {
    public interface IResult<out T>
    {
        public bool IsSuccess();
        public Error? GetError();
        public T? GetValue();
    }
}
