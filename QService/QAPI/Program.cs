using System.Net;
using System.Text;
using FluentValidation.AspNetCore;
using Grpc.Net.Client;
using MagicOnion.Client;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using QApplication;
using QApplication.Caching;
using QApplication.Interfaces;
using QApplication.Interfaces.Data;
using QApplication.Services;
using QApplication.Services.BackgroundJob;
using QApplication.Validators.AuthValidators;
using QBranchService.Contracts.Interfaces;
using QDomain.Models;
using QInfrastructure.Consumers.Cache;
using QInfrastructure.Consumers.QueueConsumers;
using QInfrastructure.Persistence.Caching;
using QInfrastructure.Persistence.DataBase;
using Serilog;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001, listenOptions => 
    { 
        listenOptions.Protocols = HttpProtocols.Http2;
    });

    options.ListenLocalhost(5003, listenOptions => 
    { 
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

builder.Services.AddSingleton(provider =>
{
    var branchServiceUrl = builder.Configuration["Services:BranchService"]
                           ?? "http://localhost:5002";

    var logger = provider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Connecting to BranchService gRPC at {Url}", branchServiceUrl);

    var channel = GrpcChannel.ForAddress(branchServiceUrl, new GrpcChannelOptions
    {
        LoggerFactory = provider.GetService<ILoggerFactory>(),
        HttpVersion = HttpVersion.Version20,
        HttpVersionPolicy = HttpVersionPolicy.RequestVersionExact
    });

    return channel;
});

builder.Services.AddSingleton<IBranchService>(provider =>
{
    var channel = provider.GetRequiredService<GrpcChannel>();
    var logger = provider.GetRequiredService<ILogger<IBranchService>>();

    logger.LogInformation("Creating MagicOnion client for IBranchValidationService");

    return MagicOnionClient.Create<IBranchService>(channel);
});

builder.Services.AddMagicOnion();


builder.Services.AddApplicationService();
builder.Services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<RegisterCustomerRequestValidator>();
});


builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IQueueCancellationService, QueueCancellationService>();
builder.Services.AddScoped<IQueueApplicationDbContext, QueueDbContext>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>()
        .GetValue<string>("Redis:ConnectionString");

    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddHostedService<QueueStartingSoonScheduler>();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CompanyCacheResetConsumer>();
    x.AddConsumer<QueueEventConsumer>();
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


builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
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

builder.Services.AddDbContext<QueueDbContext>(
    options =>
    {
        var dataSourceBuilder =
            new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
        dataSourceBuilder.EnableDynamicJson();
        var datasource = dataSourceBuilder.Build();
        options.UseNpgsql(datasource);
    });


var app = builder.Build();


app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapMagicOnionService<QueueService>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
    await db.Database.MigrateAsync();


    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserEntity>>();

    var sys = await db.Users
        .AnyAsync(u => u.EmailAddress == "systemAdmin@gmail.com");
    if (!sys)
    {
        var sysUser = new UserEntity
        {
            EmailAddress = "systemAdmin@gmail.com",
            Roles = QDomain.Enums.UserRoles.SystemAdmin,
            CreatedAt = DateTime.UtcNow,
            
        };
        sysUser.PasswordHash = hasher.HashPassword(sysUser, "B.sh.3242");
        await db.AddAsync(sysUser);
        await db.SaveChangesAsync();
    }
}

app.Run();

public partial class Program
{
}