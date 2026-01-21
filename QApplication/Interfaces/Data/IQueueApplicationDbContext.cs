using Microsoft.EntityFrameworkCore;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces.Data;

public interface IQueueApplicationDbContext
{ 
    DbSet<CompanyEntity> Companies { get; set; }
    DbSet<CustomerEntity> Customers { get; set; }
    DbSet<EmployeeEntity> Employees { get; set; }
    DbSet<AvailabilityScheduleEntity> AvailabilitySchedules { get; set; }
    DbSet<BlockedCustomerEntity> BlockedCustomers { get; set; }
    DbSet<ComplaintEntity> Complaints { get; set; }
    DbSet<QueueEntity> Queues { get; set; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    DbSet<ReviewEntity> Reviews { get; set; }
    DbSet<ServiceEntity> Services { get; set; }
    DbSet<User> Users { get; set; }
    

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}