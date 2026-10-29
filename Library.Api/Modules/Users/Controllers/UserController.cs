using Library.Api.Common.Controllers;
using Library.Api.Modules.Users.Domain;
using Library.Api.Modules.Users.Domain.Enums;
using Library.Api.Modules.Users.Dtos.Request;
using Library.Api.Modules.Users.Dtos.Response;
using Library.Api.Modules.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Modules.Users.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public sealed class UserController(UserService userService) : ApiControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Manager))]
        
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

    [HttpPatch("{id:guid}/change-user-name")]
    [Authorize(Roles = nameof(UserRole.Manager))]
    public async Task<ActionResult<UserResponse>> ChangeUserName(Guid id, ChangeUserNameRequest request )
    {
        var result = await userService.ChangeUserNameAsync(id, request.NewFullName);
        
        if (result.IsFailure)
            return ToProblem(result.Error);

        var response = ToResponse(result.Value);

        return Ok(response);
    }
    
    // the ID comes from the token
    [HttpPatch("me/password")]
    public async Task<IActionResult> ChangeMyPassword(ChangePasswordRequest request)
    {
        var userId = CurrentUserId;

        if (userId is null)
            return Unauthorized();

        var result = await userService.ChangePasswordAsync(
            userId.Value, request.CurrentPassword, request.NewPassword);

        if (result.IsFailure)
            return ToProblem(result.Error);

        return NoContent();
    }
    
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = nameof(UserRole.Manager))]
    public async Task<ActionResult<UserResponse>> DeactivateUser(Guid id)
    {
        var result = await userService.DeactivateUserAsync(id);
        
        if (result.IsFailure)
            return ToProblem(result.Error);

        var response = ToResponse(result.Value);

        return Ok(response);
    }
    
    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = nameof(UserRole.Manager))]
    public async Task<ActionResult<UserResponse>> ReactivateUser(Guid id)
    {
        var result = await userService.ReactivateUserAsync(id);
        
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
}