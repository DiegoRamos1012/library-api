using System.ComponentModel.DataAnnotations;
using Library.Api.Modules.Users.Domain.Enums;

namespace Library.Api.Modules.Users.Dtos.Request;

public record CreateUserRequest(
    [Required, StringLength(150, MinimumLength = 2)] string FullName,
    [Required, EmailAddress, StringLength(254)] string Email,
    [Required, StringLength(128, MinimumLength = 8)] string Password,
    [EnumDataType(typeof(UserRole))] UserRole Role);