using MassTransit;
using Microsoft.Extensions.Logging;
using QAggregationService.Application.Caching;
using QBranchService.Contracts.Events.BranchEvents;

namespace QAggregationService.Application.Consumers.BranchConsumers;

public class BranchCreatedConsumer: IConsumer<BranchCreatedEvent>
{
    private readonly ILogger<BranchCreatedConsumer> _logger;
    private readonly IMemoryCacheService _memoryCacheService;

    public BranchCreatedConsumer(ILogger<BranchCreatedConsumer> logger, IMemoryCacheService memoryCacheService)
    {
        _logger = logger;
        _memoryCacheService = memoryCacheService;
    }

    public Task Consume(ConsumeContext<BranchCreatedEvent> context)
    {
        _logger.LogInformation("Processing cache reset for CompanyBranches {companyId}", context.Message.CompanyId);

         _memoryCacheService.Remove(CacheKeys.BranchId(context.Message.BranchId));
         _memoryCacheService.Remove(CacheKeys.CompanyBranches(context.Message.CompanyId));
        
        _logger.LogInformation("Cache reset processed for CompanyBranches {companyId}", context.Message.CompanyId);

        return Task.CompletedTask;
    }
}