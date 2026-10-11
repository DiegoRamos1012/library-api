using Library.Api.Modules.Users.Domain;
using Library.Api.Modules.Users.Domain.Enums;

namespace Library.Api.Modules.Users.Dtos.Response;

public record UserResponse(
    Guid Id,
    string FullName,
    string Email,
    UserRole Role,
    bool IsActive);