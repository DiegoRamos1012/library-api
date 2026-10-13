using Library.Api.Common.Results;
using Library.Api.Common.Utils;
using Library.Api.Config.Security;
using Library.Api.Modules.Auth.Dtos.Request;
using Library.Api.Modules.Users.Domain;
using Library.Api.Modules.Users.Repositories;

namespace Library.Api.Modules.Auth.Services;

public class AuthService(IUserRepository userRepository, PasswordHasher passwordHasher)
{
    public async Task<Result<User>> LoginAsync(LoginRequest request)
    {
        var email = TextNormalizer.NormalizeEmail(request.Email);

        var fetchedUser = await userRepository.GetByEmailAsync(email);

        if (fetchedUser is null || !fetchedUser.IsActive ||
            !passwordHasher.VerifyPassword(request.Password, fetchedUser.PasswordHash))
        {
            return Result<User>.Failure(AuthErrors.InvalidCredentials);
        }

        return Result<User>.Success(fetchedUser);
    }
}