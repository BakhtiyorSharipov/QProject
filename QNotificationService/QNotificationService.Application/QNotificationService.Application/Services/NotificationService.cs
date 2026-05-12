using Microsoft.Extensions.Logging;
using QNotificationService.Application.Interfaces;

namespace QNotificationService.Application.Services;

public class NotificationService: INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly IEmailSender _emailSender;

    public NotificationService(ILogger<NotificationService> logger, IEmailSender emailSender)
    {
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task SendAsync(string email,int customerId, string message, CancellationToken cancellationToken)
    {

        await _emailSender.SendEmailAsync(email, "QueueNotification", message);
        
        _logger.LogInformation("Sending SMS to customer {customerId} ", customerId);
        await Task.Delay(2000, cancellationToken);
        
        _logger.LogInformation("Message: {message} sent to Customer {customerId}", message, customerId);
    }
}