using MassTransit;
using Microsoft.Extensions.Logging;
using QAggregationService.Application.Caching;
using QBranchService.Contracts.Events.BranchEvents;

namespace QAggregationService.Application.Consumers.BranchConsumers;

public class BranchUpdatedConsumer: IConsumer<BranchUpdatedEvent>
{
    private readonly ILogger<BranchUpdatedConsumer> _logger;
    private readonly IMemoryCacheService _memoryCacheService;

    public BranchUpdatedConsumer(ILogger<BranchUpdatedConsumer> logger, IMemoryCacheService memoryCacheService)
    {
        _logger = logger;
        _memoryCacheService = memoryCacheService;
    }

    public Task Consume(ConsumeContext<BranchUpdatedEvent> context)
    {
        _logger.LogInformation("Processing cache reset for CompanyBranches {companyId}", context.Message.CompanyId);

        _memoryCacheService.Remove(CacheKeys.BranchId(context.Message.BranchId));
        _memoryCacheService.Remove(CacheKeys.CompanyBranches(context.Message.CompanyId));
        
        _logger.LogInformation("Cache reset processed for CompanyBranches {companyId}", context.Message.CompanyId);
        
        return Task.CompletedTask;
        
    }
}