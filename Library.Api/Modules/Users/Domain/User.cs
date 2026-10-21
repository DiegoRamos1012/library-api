using Library.Api.Common;
using Library.Api.Common.Exceptions;
using Library.Api.Common.Utils;
using Library.Api.Modules.Users.Domain.Enums;

namespace Library.Api.Modules.Users.Domain;

public class User : BaseEntity
{
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private User(string fullName, string email, string passwordHash, UserRole role)
    {
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
    }

    public static User Create(string fullName, string email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Name is required");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required");

        return new User(
            TextNormalizer.NormalizeName(fullName),
            TextNormalizer.NormalizeEmail(email),
            passwordHash,
            role);
    }

    public void ChangeFullName(string newName)
    {
        if (!IsActive)
            throw new DomainException("Cannot change the name of an inactive user.");
        
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Name is required");

        var normalizedName = TextNormalizer.NormalizeName(newName);

        if (FullName == normalizedName)
            return;

        FullName = normalizedName;
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }
}