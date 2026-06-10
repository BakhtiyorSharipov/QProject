using MassTransit;
using Microsoft.Extensions.Logging;
using QAggregationService.Application.Caching;
using QContracts.Events;
using QContracts.QueueEvents.Enums;

namespace QAggregationService.Application.Consumers.CompanyCustomersConsumer;

public class CustomerBelongedToCompanyConsumer :IConsumer<QueueEvent>
{
    private readonly ILogger<CustomerBelongedToCompanyConsumer> _logger;
    private readonly ICacheService _cacheService;

    public CustomerBelongedToCompanyConsumer(ILogger<CustomerBelongedToCompanyConsumer> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Consume(ConsumeContext<QueueEvent> context)
    {
        _logger.LogInformation("Processing cache reset for Customer {CustomerId}", context.Message.CustomerId);

        switch (context.Message.EventType)
        {
            case QueueEventType.Created:
                await _cacheService.RemoveAsync(CacheKeys.CustomerId(context.Message.CustomerId));
                await _cacheService.RemoveAsync(CacheKeys.CompanyCustomers(context.Message.CompanyId));
                break;
            case QueueEventType.Updated:
                await _cacheService.RemoveAsync(CacheKeys.CustomerId(context.Message.CustomerId));
                await _cacheService.RemoveAsync(CacheKeys.CompanyCustomers(context.Message.CompanyId)); 
                break;
        }
        
        _logger.LogInformation("Cache reset processed for CustomerId {CustomerId}", context.Message.CustomerId);
    }
}