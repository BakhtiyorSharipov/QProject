using MassTransit;
using QNotificationService.Application;
using QNotificationService.Application.Interfaces;
using QNotificationService.Application.Services;
using QNotificationService.Application.Services.EmailSenderTypes;
using QNotificationService.Infrastructure.Consumers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<MailgunSettings>(
    builder.Configuration.GetSection("Mailgun"));
builder.Services.AddScoped<IEmailSender, MailgunEmailSender>();

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SendNotificationConsumer>();
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

builder.Services.AddLogging();

var app = builder.Build();
app.Run();