using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QAPI.IntegrationTests;
using QInfrastructure.Persistence.DataBase;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    
    private readonly RedisContainer _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();
    
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management")
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();
    private string? _postgresConnectionString;
    private string? _redisConnectionString;
    private string? _rabbitMqHost;
    private int _rabbitMqPort;
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureHostConfiguration(config =>
        {
            Dictionary<string, string?> settings = new()
            {
                
                ["ConnectionStrings:DefaultConnection"] = _postgresConnectionString,
                
                ["Redis:ConnectionString"]=_redisConnectionString,
                
                ["RabbitMQ:Host"] = _rabbitMqHost,
                ["RabbitMQ:Port"] = _rabbitMqPort.ToString(),
                ["RabbitMQ:Username"] = "guest",
                ["RabbitMQ:Password"] = "guest",
              
                ["AuthSettings:SecretKey"] =  JwtTokenTestSettings.SecretKey,
                ["AuthSettings:Audience"] = JwtTokenTestSettings.Audience,
                ["AuthSettings:Issuer"] = JwtTokenTestSettings.Issuer,
                ["Jwt:AccessTokenMinutes"] = (JwtTokenTestSettings.ExpireTimeInSeconds / 60).ToString(),
                ["Jwt:RefreshDays"] = "30"
            };

            config.AddInMemoryCollection(settings);
        });

        var host = base.CreateHost(builder);
        
   
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
            db.Database.Migrate();
        }
        
        return host;
    }

    public async Task InitializeAsync()
    {

        await Task.WhenAll(
            _postgresContainer.StartAsync(),
            _redisContainer.StartAsync(),
            _rabbitMqContainer.StartAsync()
        );
        
        
        _postgresConnectionString = _postgresContainer.GetConnectionString();
        _redisConnectionString = _redisContainer.GetConnectionString();
       
        _rabbitMqHost = _rabbitMqContainer.Hostname;
        _rabbitMqPort = _rabbitMqContainer.GetMappedPublicPort(5672);
        
        
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgresContainer.DisposeAsync().AsTask(),
            _redisContainer.DisposeAsync().AsTask(),
            _rabbitMqContainer.DisposeAsync().AsTask()
        );
        
    }
}