using Microsoft.EntityFrameworkCore;
using QDomain.Models;

namespace QInfrastructure.Persistence.DataBase;

public class QueueDbContext: DbContext
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
}