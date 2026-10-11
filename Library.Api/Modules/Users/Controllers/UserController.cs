using Library.Api.Common.Results;
using Library.Api.Modules.Users.Domain;
using Library.Api.Modules.Users.Dtos.Request;
using Library.Api.Modules.Users.Dtos.Response;
using Library.Api.Modules.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Modules.Users.Controllers;

[ApiController]
[Route("users")]
public sealed class UserController(UserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest request)
    {
        var result = await userService.CreateAsync(request);

        if (result.IsFailure)
            return ToProblem(result.Error);

        var response = ToResponse(result.Value);

        return StatusCode(
            StatusCodes.Status201Created,
            response
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetByIdAsync(Guid id)
    {
        var result = await userService.GetByIdAsync(id);
        
        if (result.IsFailure)
            return ToProblem(result.Error);

        var response = ToResponse(result.Value);

        return Ok(response);
    }
    
    private static UserResponse ToResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.IsActive
        );
    }
    
    
    private ObjectResult ToProblem(ResultError error)
    {
        var statusCode = error.Code switch
        {
            "USER_EMAIL_ALREADY_EXISTS" => StatusCodes.Status409Conflict,
            "USER_INVALID_INPUT" => StatusCodes.Status400BadRequest,
            "USER_NOT_FOUND" => StatusCodes.Status404NotFound,
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