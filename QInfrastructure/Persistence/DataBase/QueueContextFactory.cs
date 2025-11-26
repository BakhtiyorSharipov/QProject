using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QInfrastructure.Persistence.DataBase;

public class QueueContextFactory:IDesignTimeDbContextFactory<QueueDbContext>
{
    public QueueDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<QueueDbContext>();
        optionBuilder.UseNpgsql("Host=localhost;Port=5432;Database=WayToSuccess;Username=postgres;Password=2415");
        return new QueueDbContext(optionBuilder.Options);
    }
}