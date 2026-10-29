using Library.Api.Common.Results;

namespace Library.Api.Modules.Users;

public static class UserErrors
{
    public static readonly ResultError EmailAlreadyExists =
        new("USER_EMAIL_ALREADY_EXISTS", "Email already exists");

    public static readonly ResultError InvalidCurrentPassword =
        new("USER_INVALID_CURRENT_PASSWORD", "Invalid current password");
    
    public static readonly ResultError SamePassword = new(
        "USER_SAME_PASSWORD", "The new password must be different from the current password.");

    public static readonly ResultError NotFound =
        new("USER_NOT_FOUND", "User not found");
    
    public static readonly ResultError UserDeactivated = 
        new("USER_DEACTIVATED", "Cannot update a deactivated user");

    public static readonly ResultError UserAlreadyDeactivated =
        new("USER_ALREADY_DEACTIVATED", "User already deactivated");
    
    public static readonly ResultError UserAlreadyActivated =
        new("USER_ALREADY_ACTIVATED", "User already activated");
    
}