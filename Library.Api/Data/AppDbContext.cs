using Library.Api.Common;
using Library.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    TimeProvider timeProvider
) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly
        );
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(e => e.CreatedAt).CurrentValue = now.UtcDateTime;
                    entry.Property(e => e.UpdatedAt).CurrentValue = now.UtcDateTime;
                    break;

                case EntityState.Modified:
                    entry.Property(e => e.UpdatedAt).CurrentValue = now.UtcDateTime;
                    break;

                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}