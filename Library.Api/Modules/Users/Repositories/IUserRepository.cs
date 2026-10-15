using Library.Api.Modules.Users.Domain;

namespace Library.Api.Modules.Users.Repositories;

// Com a interface, podemos testar os serviços sem precisar de banco de dados
public interface IUserRepository
{
    Task<bool> ExistsByIdAsync(Guid id);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}