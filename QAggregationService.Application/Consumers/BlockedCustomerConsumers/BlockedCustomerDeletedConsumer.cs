using MassTransit;
using Microsoft.Extensions.Logging;
using QAggregationService.Application.Caching;
using QContracts.Events.BlockedCustomerEvent;

namespace QAggregationService.Application.Consumers.BlockedCustomerConsumers;

public class BlockedCustomerDeletedConsumer: IConsumer<BlockedCustomerDeletedEvent>
{
    private readonly ILogger<BlockedCustomerDeletedConsumer> _logger;
    private readonly ICacheService _cacheService;

    public BlockedCustomerDeletedConsumer(ILogger<BlockedCustomerDeletedConsumer> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Consume(ConsumeContext<BlockedCustomerDeletedEvent> context)
    {
        _logger.LogInformation("Processing cache reset for BlockedCustomer {BlockedCustomerId}", context.Message.BlockedCustomerId);

        await _cacheService.RemoveAsync(CacheKeys.BlockedCustomerId(context.Message.BlockedCustomerId));
        await _cacheService.RemoveAsync(CacheKeys.CompanyBlockedCustomers(context.Message.BlockedCustomerId));
        
        _logger.LogInformation("Cache reset processed for BlockedCustomerId {BlockedCustomerId}", context.Message.BlockedCustomerId);
    }
}