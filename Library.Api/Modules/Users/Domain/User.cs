using Library.Api.Common;
using Library.Api.Modules.Users.Domain.Enums;

namespace Library.Api.Modules.Users.Domain;

public class User(string fullName, string email, string passwordHash, UserRole role, bool isActive)
    : BaseEntity
{
    public string FullName { get; private set; } = fullName;
    public string Email { get; private set; } = email;
    public string PasswordHash { get; private set; } = passwordHash;
    public UserRole Role { get; private set; } = role;
    public bool IsActive { get; private set; } = isActive;
}