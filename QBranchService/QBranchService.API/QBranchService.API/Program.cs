using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using QBranchService.Application;
using QBranchService.Application.Helpers;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Validators.CompanyValidators;
using QBranchService.Infrastructure.Persistence.DataBase;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5002, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });

    options.ListenLocalhost(5003, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
});

builder.Services.AddFluentValidation(fv => 
{
    fv.RegisterValidatorsFromAssemblyContaining<CreateCompanyValidator>();
});


builder.Services.AddMagicOnion();

builder.Services.AddApplicationService();


builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter()); });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBranchServiceApplicationDbContext, BranchServiceDbContext>();


builder.Services.AddDbContext<BranchServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

app.MapMagicOnionService();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();