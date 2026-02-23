using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QBranchService.Infrastructure.Persistence.DataBase;

public class BranchContextFactory:IDesignTimeDbContextFactory<BranchServiceDbContext>
{
    public BranchServiceDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<BranchServiceDbContext>();
        optionBuilder.UseNpgsql("Host=localhost; Port=5432; Database=QBranchService; Username=postgres; Password=b.sh.3242");
        return new BranchServiceDbContext(optionBuilder.Options);
    }
}