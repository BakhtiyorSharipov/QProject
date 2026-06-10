using MassTransit;
using Microsoft.Extensions.Logging;
using QAggregationService.Application.Caching;
using QContracts.Events.CustomerEvent;

namespace QAggregationService.Application.Consumers.CustomerConsumers;

public class CustomerUpdatedConsumer: IConsumer<CustomerUpdatedEvent>
{
    private readonly ILogger<CustomerUpdatedConsumer> _logger;
    private readonly ICacheService _cacheService;

    public CustomerUpdatedConsumer(ILogger<CustomerUpdatedConsumer> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Consume(ConsumeContext<CustomerUpdatedEvent> context)
    {
        _logger.LogInformation("Processing cache reset for EmployeeId {EmployeeId}", context.Message.CustomerId);

        await _cacheService.RemoveAsync(CacheKeys.EmployeeId(context.Message.CustomerId));
        await _cacheService.RemoveAsync(CacheKeys.AllCustomers());
        
        _logger.LogInformation("Cache reset processed for EmployeeId {EmployeeId}", context.Message.CustomerId);
    }
}