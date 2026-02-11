using System.Text;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
using QDomain.Models;
using QInfrastructure.Consumers.Cache;
using QInfrastructure.Consumers.Queue;
using QInfrastructure.Persistence.Caching;
using QInfrastructure.Persistence.DataBase;
using Serilog;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationService();
builder.Services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<RegisterCustomerRequestValidator>();
});


builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IQueueApplicationDbContext, QueueDbContext>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>()
        .GetValue<string>("Redis:ConnectionString");

    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddHostedService<QueueStartingSoonScheduler>();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<QueueBookedConsumer>();
    x.AddConsumer<QueueCanceledByCustomerConsumer>();
    x.AddConsumer<QueueCanceledByAdminConsumer>();
    x.AddConsumer<QueueCanceledByEmployeeConsumer>();
    x.AddConsumer<QueueCompletedConsumer>();
    x.AddConsumer<QueueConfirmedConsumer>();
    x.AddConsumer<QueueStartingSoonConsumer>();
    x.AddConsumer<CacheResetConsumer>();
    x.AddConsumer<CompanyCacheResetConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]);
            h.Password(builder.Configuration["RabbitMQ:Password"]);
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
            new string[]{ }
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
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
   await db.Database.MigrateAsync();

    var userRepo = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    var sys = await db.Users
        .AnyAsync(u => u.EmailAddress == "systemAdmin@gmail.com");
    if (!sys)
    {
        var sysUser = new User
        {
                EmailAddress = "systemAdmin@gmail.com",
            Roles = QDomain.Enums.UserRoles.SystemAdmin,
            CreatedAt = DateTime.UtcNow
        };
        sysUser.PasswordHash = hasher.HashPassword(sysUser, "B.sh.3242"); 
        await userRepo.AddAsync(sysUser);
        await userRepo.SaveChangesAsync();
    }
}
app.Run();

public partial class Program
{
}