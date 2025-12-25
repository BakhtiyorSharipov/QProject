using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Data;
using QDomain.Models;

namespace QInfrastructure.Persistence.DataBase;

public class QueueDbContext: DbContext, IQueueApplicationDbContext
{
    public QueueDbContext(DbContextOptions<QueueDbContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost; Port=5432; Database=QProject; Username=postgres; Password=b.sh.3242");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<BaseEntity>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TableConfiguration.CustomerTableConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<CompanyEntity> Companies { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<AvailabilityScheduleEntity> AvailabilitySchedules { get; set; }
    public DbSet<BlockedCustomerEntity> BlockedCustomers { get; set; }
    public DbSet<ComplaintEntity> Complaints { get; set; }
    public DbSet<QueueEntity> Queues { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<ReviewEntity> Reviews { get; set; }
    public DbSet<ServiceEntity> Services { get; set; }
    public DbSet<User> Users { get; set; }
    
    
}