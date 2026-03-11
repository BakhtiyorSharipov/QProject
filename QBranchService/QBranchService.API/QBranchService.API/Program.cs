using MassTransit;
using Microsoft.EntityFrameworkCore;
using QBranchService.Application;
using QBranchService.Application.Consumers;
using QBranchService.Application.Helpers;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Infrastructure.Persistence.DataBase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationService();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ValidateBranchIdsConsumer>();
    x.AddConsumer<ValidateCompanyConsumer>();
    x.AddConsumer<ValidateCompanyServiceConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        var config = builder.Configuration.GetSection("RabbitMQ");
        cfg.Host(config["Host"], rabbitMqHostConfigurator =>
        {
            rabbitMqHostConfigurator.Username(config["Username"]!);
            rabbitMqHostConfigurator.Password(config["Password"]!);
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter()); });
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