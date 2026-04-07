using System.Net;
using MagicOnion.Client;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using QAggregationService.Application.Services;
using QAggregationService.Contracts.Interfaces;
using Grpc.Net.Client;
using MassTransit;
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

builder.Services.AddSingleton<IQueueService>(provider =>
{
    var url = builder.Configuration["Services:QueueService"] 
              ?? "http://localhost:5001";

    var logger = provider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Connecting to QueueService gRPC at {Url}", url);

    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
    {
        LoggerFactory = provider.GetService<ILoggerFactory>(),
        HttpHandler = handler,
        HttpVersion = HttpVersion.Version20,
        HttpVersionPolicy = HttpVersionPolicy.RequestVersionExact
    });

    return MagicOnionClient.Create<IQueueService>(channel);
});

builder.Services.AddSingleton<IBranchService>(provider =>
{
    var url = builder.Configuration["Services:BranchService"] 
              ?? "http://localhost:5002";

    var logger = provider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Connecting to BranchService gRPC at {Url}", url);

    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
    {
        LoggerFactory = provider.GetService<ILoggerFactory>(),
        HttpHandler = handler,
        HttpVersion = HttpVersion.Version20,
        HttpVersionPolicy = HttpVersionPolicy.RequestVersionExact
    });

    return MagicOnionClient.Create<IBranchService>(channel);
});


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
builder.Services.AddSwaggerGen();



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