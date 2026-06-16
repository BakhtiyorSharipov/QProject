using Microsoft.EntityFrameworkCore;
using QUserService.Application.Interfaces;
using QUserService.Domain.Models;
using QUserService.Infrastructure.Persistence.TableConfiguration;

namespace QUserService.Infrastructure.Persistence.Database;

public class UserServiceDbContext : DbContext, IUserServiceApplicationDbContext
{
    public UserServiceDbContext(DbContextOptions<UserServiceDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<BaseEntity>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserTableConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }


    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<CustomerEntity> Customer { get; set; }
    public DbSet<BlockedCustomerEntity> BlockedCustomers { get; set; }
    public DbSet<AvailabilityScheduleEntity> AvailabilitySchedules { get; set; }
}