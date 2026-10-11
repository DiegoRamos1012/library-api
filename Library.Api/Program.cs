using Library.Api.Config.Security;
using Library.Api.Data;
using Library.Api.Modules.Users.Repositories;
using Library.Api.Modules.Users.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registro de serviços
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<UserService>();

builder.Services.AddControllers();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PasswordHasher>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Configuração de pipeline e ativação
app.MapControllers();

app.Run();