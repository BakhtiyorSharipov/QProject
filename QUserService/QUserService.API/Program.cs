using Microsoft.EntityFrameworkCore;
using QUserService.Infrastructure.Persistence.Database;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<UserServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();