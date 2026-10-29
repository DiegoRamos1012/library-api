using System.Security.Claims;
using Library.Api.Common.Results;
using Library.Api.Modules.Users.Domain.Enums;
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

            "USER_EMAIL_ALREADY_EXISTS" or "USER_ALREADY_DEACTIVATED"
                or "USER_ALREADY_ACTIVATED" or "USER_DEACTIVATED"
                => StatusCodes.Status409Conflict,

            "USER_INVALID_INPUT" or "USER_SAME_PASSWORD"
                or "USER_INVALID_CURRENT_PASSWORD"
                => StatusCodes.Status400BadRequest,

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
    
    protected Guid? CurrentUserId =>
        Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"),
            out var id) ? id : null;
    
    // Método auxiliar que controla se um usuário pode alterar outro
    /*
     protected bool CanManageUser(Guid targetUserId)
    {
        if (User.IsInRole(nameof(UserRole.Manager)))
            return true;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim?.Value, out var currentUserId)
               && currentUserId == targetUserId;
    } 
    */
}