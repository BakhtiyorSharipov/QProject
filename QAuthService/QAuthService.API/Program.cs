using Microsoft.EntityFrameworkCore;
using QAuthService.Infrastructure.Persistence.Database;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AuthServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();