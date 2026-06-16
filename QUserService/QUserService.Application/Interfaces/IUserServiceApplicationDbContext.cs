using Microsoft.EntityFrameworkCore;
using QUserService.Domain.Models;

namespace QUserService.Application.Interfaces;

public interface IUserServiceApplicationDbContext
{
    DbSet<UserEntity> Users { get; set; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    
    DbSet<EmployeeEntity> Employees { get; set; }
    DbSet<CustomerEntity> Customer { get; set; }
    DbSet<BlockedCustomerEntity> BlockedCustomers { get; set; }
    DbSet<AvailabilityScheduleEntity> AvailabilitySchedules { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
}