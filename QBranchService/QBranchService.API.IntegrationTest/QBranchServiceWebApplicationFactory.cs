using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QBranchService.Infrastructure.Persistence.DataBase;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace QBranchService.API.IntegrationTest;

public class QBranchServiceWebApplicationFactory: WebApplicationFactory<Program>, IAsyncLifetime
{
    
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("branch_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management")
        .WithUsername("guest")
        .WithPassword("guest")
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
                
                ["RabbitMQ:Host"] = _rabbitMqContainer.Hostname,
                ["RabbitMQ:Port"] = _rabbitMqContainer.GetMappedPublicPort(5672).ToString(),
                ["RabbitMQ:Username"] = "guest",
                ["RabbitMQ:Password"] = "guest",
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
        await Task.WhenAll(
            _postgresContainer.StartAsync(),
            _rabbitMqContainer.StartAsync()
        );
        
        _postgresConnectionString = _postgresContainer.GetConnectionString();

    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgresContainer.DisposeAsync().AsTask(),
            _rabbitMqContainer.DisposeAsync().AsTask()
        );

    }
    
    
}