using Microsoft.EntityFrameworkCore;
using Npgsql;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Infrastructure.Persistence.DataBase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBranchServiceApplicationDbContext, BranchServiceDbContext>();


builder.Services.AddDbContext<BranchServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();