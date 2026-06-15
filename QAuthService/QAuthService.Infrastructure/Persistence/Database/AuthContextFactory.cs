using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QAuthService.Infrastructure.Persistence.Database;

public class AuthContextFactory: IDesignTimeDbContextFactory<AuthServiceDbContext>
{
    public AuthServiceDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<AuthServiceDbContext>();
        optionBuilder.UseNpgsql(
            "Host=localhost; Port=5432; Database=QAuthService; Username=postgres; Password=b.sh.3242");
        return new AuthServiceDbContext(optionBuilder.Options);
    }
}