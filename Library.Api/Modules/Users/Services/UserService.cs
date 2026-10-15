using Library.Api.Common.Exceptions;
using Library.Api.Common.Results;
using Library.Api.Common.Utils;
using Library.Api.Config.Security;
using Library.Api.Modules.Users.Domain;
using Library.Api.Modules.Users.Dtos.Request;
using Library.Api.Modules.Users.Repositories;

namespace Library.Api.Modules.Users.Services;

public class UserService (IUserRepository userRepository, PasswordHasher passwordHasher)
{
    public async Task<Result<User>> CreateAsync(CreateUserRequest request)
    {
        
        var email = TextNormalizer.NormalizeEmail(request.Email);
        
        if (await userRepository.ExistsByEmailAsync(email))
        {
            return Result<User>.Failure(UserErrors.EmailAlreadyExists);
        }

        var passwordHash = passwordHasher.PasswordEncoder(request.Password);
        
        User newUser;

        try
        {
            newUser = User.Create(
                fullName: request.FullName,
                email: email,
                passwordHash: passwordHash,
                role: request.Role);
        }
        catch (DomainException ex)
        {
            return Result<User>.Failure("USER_INVALID_INPUT", ex.Message);
        }

        await userRepository.AddAsync(newUser);

        return Result<User>.Success(newUser);
    }

    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        var fetchedUser = await userRepository.GetByIdAsync(id);

        return fetchedUser is null ? Result<User>.Failure(UserErrors.NotFound) : Result<User>.Success(fetchedUser);
    }

    public async Task<Result<User>> DeactivateUserAsync(Guid id)
    {
        var fetchedUser = await userRepository.GetByIdAsync(id);

        if (fetchedUser is null)
            return Result<User>.Failure(UserErrors.NotFound);

        if (!fetchedUser.IsActive)
            return Result<User>.Failure(UserErrors.UserAlreadyDeactivated);

        fetchedUser.Deactivate();

        await userRepository.SaveChangesAsync();

        return Result<User>.Success(fetchedUser);
    }
    
    public async Task<Result<User>> ReactivateUserAsync(Guid id)
    {
        var fetchedUser = await userRepository.GetByIdAsync(id);
        if (fetchedUser is null)
            return Result<User>.Failure(UserErrors.NotFound);
        
        if (fetchedUser.IsActive)
            return Result<User>.Failure(UserErrors.UserAlreadyActivated);

        fetchedUser.Reactivate();

        await userRepository.SaveChangesAsync();

        return Result<User>.Success(fetchedUser);
    }
    
    
}