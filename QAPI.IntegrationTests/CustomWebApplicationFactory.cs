using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QAPI.IntegrationTests;
using QInfrastructure.Persistence.DataBase;
using Testcontainers.PostgreSql;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private string? _connectionString;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureHostConfiguration(config =>
        {
            Dictionary<string, string?> settings = new()
            {
                
                ["ConnectionStrings:DefaultConnection"] = _connectionString,

              
                ["AuthSettings:SecretKey"] =  JwtTokenTestSettings.SecretKey,
                ["AuthSettings:Audience"] = JwtTokenTestSettings.Audience,
                ["AuthSettings:Issuer"] = JwtTokenTestSettings.Issuer,
                ["Jwt:AccessTokenMinutes"] = (JwtTokenTestSettings.ExpireTimeInSeconds / 60).ToString(),
                ["Jwt:RefreshDays"] = "30"
            };

            config.AddInMemoryCollection(settings);
        });

        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        _connectionString = _postgresContainer.GetConnectionString();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }
}