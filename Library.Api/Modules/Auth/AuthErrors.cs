using Library.Api.Common.Results;

namespace Library.Api.Modules.Auth;

public static class AuthErrors
{
    public static readonly ResultError InvalidCredentials =
        new("AUTH_INVALID_CREDENTIALS", "Invalid email or password");
}