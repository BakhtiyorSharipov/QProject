using MassTransit;
using Microsoft.Extensions.Logging;
using QAggregationService.Application.Caching;
using QContracts.Events.EmployeeEvent;

namespace QAggregationService.Application.Consumers.EmployeeConsumers;

public class EmployeeDeletedConsumer: IConsumer<EmployeeDeletedEvent>
{
    private readonly ILogger<EmployeeDeletedConsumer> _logger;
    private readonly ICacheService _cacheService;

    public EmployeeDeletedConsumer(ILogger<EmployeeDeletedConsumer> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }
    public async Task Consume(ConsumeContext<EmployeeDeletedEvent> context)
    {
        _logger.LogInformation("Processing cache reset for EmployeeId {EmployeeId}", context.Message.EmployeeId);

        await _cacheService.RemoveAsync(CacheKeys.EmployeeId(context.Message.EmployeeId));
        await _cacheService.RemoveAsync(CacheKeys.CompanyEmployees(context.Message.CompanyId));
        await _cacheService.RemoveAsync(CacheKeys.AllEmployees());
        
        _logger.LogInformation("Cache reset processed for EmployeeId {EmployeeId}", context.Message.EmployeeId);
    }
}