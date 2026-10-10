using Library.Api.Modules.Users.Domain;
using Library.Api.Modules.Users.Repositories;

namespace Library.Api.Modules.Users.Services;

public class UserService : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(User user)
    {
        throw new NotImplementedException();
    }
}