using Blockify.Api.Controllers.Communication;
using Microsoft.AspNetCore.Mvc;

namespace Blockify.Api.Controllers
{
    public static class ControllerBaseExtension
    {
        public static IActionResult ErrorResponse(this ControllerBase _base, int statusCode, Error error)
        {
            return _base.StatusCode(
                statusCode,
                ResponseModel<object>.Fail(error));
        }
    }
}
