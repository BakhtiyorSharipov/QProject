using Microsoft.Extensions.Logging;
using QApplication.Interfaces;

namespace QApplication.Services;

public class SmsService: ISmsService
{
    private readonly ILogger<SmsService> _logger;

    public SmsService(ILogger<SmsService> logger)
    {
        _logger = logger;
    }

    public async Task Send(int customerId, string message, CancellationToken cancellationToken)
    {
        
        _logger.LogInformation("Sending SMS to customer {customerId} ", customerId);
        await Task.Delay(2000, cancellationToken);
        
       _logger.LogInformation("Message {message} sent to Customer {customerId}", message, customerId);
    }
    
    
    
}