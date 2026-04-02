using System.Net;
using MagicOnion;
using MagicOnion.Client;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using QAggregationService.Application.Services;
using QAggregationService.Contracts.Interfaces;
using Grpc.Net.Client;
using QAggregationService.Application.Caching;
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

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>()
        .GetValue<string>("Redis:ConnectionString");

    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddScoped<IAggregationService, AggregationService>();

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