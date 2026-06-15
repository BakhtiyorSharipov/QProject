using Microsoft.EntityFrameworkCore;
using QAuthService.Application.Interfaces;
using QAuthService.Domain.Models;
using QAuthService.Infrastructure.Persistence.TableConfiguration;

namespace QAuthService.Infrastructure.Persistence.Database;

public class AuthServiceDbContext: DbContext, IAuthServiceApplicationDbContext
{

    public AuthServiceDbContext(DbContextOptions<AuthServiceDbContext> options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserTableConfiguration).Assembly);
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
}