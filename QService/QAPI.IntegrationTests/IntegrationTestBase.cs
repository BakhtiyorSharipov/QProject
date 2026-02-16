using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using QApplication.Interfaces;
using QDomain.Enums;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;


namespace QAPI.IntegrationTests;

public abstract class IntegrationTestBase: IAsyncLifetime, IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task<string> GetJwtToken(string role)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var passwordHasher = new PasswordHasher<User>();
        
        var user = new User
        {
            EmailAddress = $"test_{Guid.NewGuid()}@test.com",
            Roles = Enum.Parse<UserRoles>(role),
            CreatedAt = DateTime.UtcNow,
            
        };

        user.PasswordHash = passwordHasher.HashPassword(user, "B.sh.3242");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        return tokenService.GenerateAccessToken(user.Id, user.Employee?.FirstName ?? "Test", user.Roles.ToString(), expiresAt);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}