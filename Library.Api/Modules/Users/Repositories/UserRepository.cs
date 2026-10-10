using Library.Api.Data;
using Library.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Modules.Users.Repositories;

public class UserRepository(AppDbContext ctx) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
    }
}