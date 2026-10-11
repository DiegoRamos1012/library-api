using Library.Api.Common.Exceptions;
using Library.Api.Common.Utils;
using Library.Api.Config.Security;
using Library.Api.Modules.Users.Domain;
using Library.Api.Modules.Users.Dtos.Request;
using Library.Api.Modules.Users.Repositories;


namespace Library.Api.Modules.Users.Services;

public class UserService (IUserRepository userRepository, PasswordHasher passwordHasher)
{
    public async Task<User> CreateAsync(CreateUserRequest dto)
    {
        
        var email = TextNormalizer.NormalizeEmail(dto.Email);
        
        if (await userRepository.ExistsByEmailAsync(email))
        {
            throw new BusinessException.EmailAlreadyExistsException();
        }

        var passwordHash = passwordHasher.PasswordEncoder(dto.Password);
        
        var newUser = User.Create(
            fullName: dto.FullName,
            email: email,
            passwordHash: passwordHash,
            role: dto.Role
        );
        
        await userRepository.AddAsync(newUser);

        return newUser;
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);

        return user ?? throw new BusinessException.UserNotFoundException();
    }
}