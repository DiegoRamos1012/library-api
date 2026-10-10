using Library.Api.Modules.Users.Domain;

namespace Library.Api.Modules.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
}