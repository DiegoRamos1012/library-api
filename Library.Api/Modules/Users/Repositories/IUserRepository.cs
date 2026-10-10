using Library.Api.Modules.Users.Domain;

namespace Library.Api.Modules.Users.Repositories;

// Com a interface, podemos testar os serviços sem precisar de banco de dados
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
}