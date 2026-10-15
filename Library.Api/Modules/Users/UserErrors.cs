using Library.Api.Common.Results;

namespace Library.Api.Modules.Users;

public static class UserErrors
{
    public static readonly ResultError EmailAlreadyExists =
        new("USER_EMAIL_ALREADY_EXISTS", "Email already exists");

    public static readonly ResultError NotFound =
        new("USER_NOT_FOUND", "User not found");

    public static readonly ResultError UserAlreadyDeactivated =
        new("USER_ALREADY_DEACTIVATED", "User already deactivated");
    
    public static readonly ResultError UserAlreadyActivated =
        new("USER_ALREADY_ACTIVATED", "User already activated");
    
}