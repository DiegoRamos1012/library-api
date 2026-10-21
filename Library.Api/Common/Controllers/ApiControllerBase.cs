using Library.Api.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Common.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ObjectResult ToProblem(ResultError error)
    {
        var statusCode = error.Code switch
        {
            "USER_NOT_FOUND" => StatusCodes.Status404NotFound,
            "USER_EMAIL_ALREADY_EXISTS" or "USER_ALREADY_DEACTIVATED" or "USER_ALREADY_ACTIVATED" or "USER_DEACTIVATED" 
                => StatusCodes.Status409Conflict,
            "USER_INVALID_INPUT" => StatusCodes.Status400BadRequest,
            "AUTH_INVALID_CREDENTIALS" => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
        
        return Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Message,
            extensions: error.Details is null
                ? null
                : new Dictionary<string, object?>
                {
                    ["details"] = error.Details
                }
        );
    }
}