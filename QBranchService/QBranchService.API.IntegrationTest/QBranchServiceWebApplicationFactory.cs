using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QBranchService.Infrastructure.Persistence.DataBase;
using Testcontainers.PostgreSql;

namespace QBranchService.API.IntegrationTest;

public class QBranchServiceWebApplicationFactory: WebApplicationFactory<Program>, IAsyncLifetime
{
    
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("branch_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    
    private string? _postgresConnectionString;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureHostConfiguration(config =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgresConnectionString,
            };

            config.AddInMemoryCollection(settings);
        });
        
        var host = base.CreateHost(builder);
        
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BranchServiceDbContext>(); 
            db.Database.Migrate();
        }
        
        return host;


    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        _postgresConnectionString = _postgresContainer.GetConnectionString();

    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync().AsTask();

    }
    
    
}