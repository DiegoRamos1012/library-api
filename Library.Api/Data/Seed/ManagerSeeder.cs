using Library.Api.Modules.Users.Domain.Enums;
using Library.Api.Modules.Users.Dtos.Request;
using Library.Api.Modules.Users.Services;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Data.Seed;

public class ManagerSeeder(AppDbContext context, UserService userService, IConfiguration configuration)
{
    public async Task SeedAsync()
    {
        if (await context.Users.AnyAsync())
            return;

        var fullName = configuration["Seed:Manager:FullName"];
        var email = configuration["Seed:Manager:Email"];
        var password = configuration["Seed:Manager:Password"];

        if (string.IsNullOrWhiteSpace(fullName)
            || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password))
            return;

        var request = new CreateUserRequest(fullName, email, password, UserRole.Manager);
        var result = await userService.CreateAsync(request);

        if (result.IsFailure)
            throw new InvalidOperationException($"Manager seed failed: {result.Error.Message}");
    }
}