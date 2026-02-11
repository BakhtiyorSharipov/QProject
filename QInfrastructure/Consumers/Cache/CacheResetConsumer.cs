using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QContracts.CashingEvents;

namespace QInfrastructure.Consumers.Cache;

public class CacheResetConsumer: IConsumer<CacheResetEvent>
{
    private readonly ICacheService _cache;
    private readonly ILogger<CacheResetConsumer> _logger;

    public CacheResetConsumer(ICacheService cache, ILogger<CacheResetConsumer> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CacheResetEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("Processing cache event for QueueId {QueueId}", evt.QueueId);

        await _cache.HashRemoveAsync(CacheKeys.AllQueuesHashKey, context.CancellationToken);
        await _cache.HashRemoveAsync(CacheKeys.CustomerQueuesHashKey(evt.CustomerId), context.CancellationToken);
        await _cache.RemoveAsync(CacheKeys.QueueId(evt.QueueId));
        await _cache.RemoveAsync(CacheKeys.EmployeeId(evt.EmployeeId));
        
        _logger.LogInformation("Cache event processed for QueueId {QueueId}", evt.QueueId);
    }
}