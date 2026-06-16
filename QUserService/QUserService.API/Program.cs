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
using QBranchService.Contracts.Interfaces;
using QUserService.Application.Caching;
using QUserService.Application.Extensions;
using QUserService.Application.Interfaces;
using QUserService.Application.Services;
using QUserService.Application.Validators.AuthValidators;
using QUserService.Domain.Models;
using QUserService.Infrastructure.Persistence.Caching;
using QUserService.Infrastructure.Persistence.Database;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5007, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });

    options.ListenLocalhost(5008, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
});

var branchServiceUrl = builder.Configuration["Services:BranchService"]
                       ?? "http://localhost:5002";
builder.Services.AddSingleton<IBranchService>(_ =>
    MagicOnionClient.Create<IBranchService>(GrpcChannel.ForAddress(branchServiceUrl)));



builder.Services.AddMagicOnion();

builder.Services.AddApplicationService();
builder.Services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<RegisterCustomerRequestValidator>();
});


builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserServiceApplicationDbContext, UserServiceDbContext>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>()
        .GetValue<string>("Redis:ConnectionString");

    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();


builder.Services.AddMassTransit(x =>
{
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


builder.Services.AddDbContext<UserServiceDbContext>(options =>
{
    var dataSourceBuilder =
        new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
    dataSourceBuilder.EnableDynamicJson();
    var datasource = dataSourceBuilder.Build();
    options.UseNpgsql(datasource);
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.MapMagicOnionService<UserService>();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserServiceDbContext>();
    await db.Database.MigrateAsync();


    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserEntity>>();

    var sys = await db.Users
        .AnyAsync(u => u.EmailAddress == "systemAdmin@gmail.com");
    if (!sys)
    {
        var sysUser = new UserEntity
        {
            EmailAddress = "systemAdmin@gmail.com",
            Roles = QUserService.Domain.Enums.UserRoles.SystemAdmin,
            CreatedAt = DateTime.UtcNow,
        };
        sysUser.PasswordHash = hasher.HashPassword(sysUser, "B.sh.3242");
        await db.AddAsync(sysUser);
        await db.SaveChangesAsync();
    }
}

app.MapGet("/", () => "Hello World!");

app.Run();