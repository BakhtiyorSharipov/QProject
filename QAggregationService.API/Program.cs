using System.Text;
using MagicOnion.Client;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using QAggregationService.Application.Services;
using QAggregationService.Contracts.Interfaces;
using Grpc.Net.Client;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QAggregationService.Application.Caching;
using QAggregationService.Application.Consumers.BlockedCustomerConsumers;
using QAggregationService.Application.Consumers.BranchConsumers;
using QAggregationService.Application.Consumers.CompanyConsumers;
using QAggregationService.Application.Consumers.CompanyCustomersConsumer;
using QAggregationService.Application.Consumers.CompanyServiceConsumers;
using QAggregationService.Application.Consumers.CustomerConsumers;
using QAggregationService.Application.Consumers.EmployeeConsumers;
using QContracts.Interfaces;
using QBranchService.Contracts.Interfaces;
using QUserService.Contracts.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5004, listenOptions => 
    { 
        listenOptions.Protocols = HttpProtocols.Http2;
    });

    options.ListenLocalhost(5005, listenOptions => 
    { 
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

builder.Services.AddMagicOnion();

var branchServiceUrl = builder.Configuration["Services:BranchService"]
                       ?? "http://localhost:5002";
builder.Services.AddSingleton<IBranchService>(_ =>
    MagicOnionClient.Create<IBranchService>(GrpcChannel.ForAddress(branchServiceUrl)));


var userServiceUrl = builder.Configuration["Services:UserService"]
                     ?? "http://localhost:5007";
builder.Services.AddSingleton<IUserService>(_ =>
    MagicOnionClient.Create<IUserService>(GrpcChannel.ForAddress(userServiceUrl)));

var queueServiceUrl = builder.Configuration["Services:QueueService"] 
          ?? "http://localhost:5001";
builder.Services.AddSingleton<IQueueService>(_ =>
    MagicOnionClient.Create<IQueueService>(GrpcChannel.ForAddress(queueServiceUrl)));



builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>()
        .GetValue<string>("Redis:ConnectionString");

    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IMemoryCacheService, MemoryCacheService>();

builder.Services.AddScoped<IAggregationService, AggregationService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmployeeCreatedConsumer>();
    x.AddConsumer<EmployeeUpdatedConsumer>();
    x.AddConsumer<EmployeeDeletedConsumer>();
    x.AddConsumer<CustomerBelongedToCompanyConsumer>();
    x.AddConsumer<CustomerCreatedConsumer>();
    x.AddConsumer<CustomerDeletedConsumer>();
    x.AddConsumer<CustomerUpdatedConsumer>();
    x.AddConsumer<BlockedCustomerCreatedConsumer>();
    x.AddConsumer<BlockedCustomerDeletedConsumer>();
    x.AddConsumer<CompanyCreatedConsumer>();
    x.AddConsumer<CompanyUpdatedConsumer>();
    x.AddConsumer<CompanyDeletedConsumer>();
    x.AddConsumer<BranchCreatedConsumer>();
    x.AddConsumer<BranchUpdatedConsumer>();
    x.AddConsumer<BranchDeletedConsumer>();
    x.AddConsumer<CompanyServiceCreatedConsumer>();
    x.AddConsumer<CompanyServiceUpdatedConsumer>();
    x.AddConsumer<CompanyServiceDeletedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetService<IConfiguration>();
        
        var host = configuration?["RabbitMQ:Host"] ?? "localhost";
        var port = configuration?.GetValue<ushort?>("RabbitMQ:Port") ?? 5672;
        var username = configuration?["RabbitMQ:Username"] ?? "guest";
        var password = configuration?["RabbitMQ:Password"] ?? "guest";
        
        cfg.Host(host, port, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });
        
        cfg.ConfigureEndpoints(context);
        
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapControllers();

app.Run();

public partial class Program { }