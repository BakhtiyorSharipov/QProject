using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QUserService.Infrastructure.Persistence.Database;

public class UserServiceContextFactory: IDesignTimeDbContextFactory<UserServiceDbContext>
{
    public UserServiceDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<UserServiceDbContext>();
        optionBuilder.UseNpgsql("Host=localhost;Port=5432;Database=QUserService;Username=postgres;Password=b.sh.3242");
        return new UserServiceDbContext(optionBuilder.Options);
    }
}