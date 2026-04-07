using MassTransit;
using Microsoft.Extensions.Logging;
using QAggregationService.Application.Caching;
using QBranchService.Contracts.Events.CompanyEvents;

namespace QAggregationService.Application.Consumers.CompanyConsumers;

public class CompanyDeletedConsumer: IConsumer<CompanyDeletedEvent>
{
    private readonly ILogger<CompanyDeletedConsumer> _logger;
    private readonly IMemoryCacheService _memoryCacheService;

    public CompanyDeletedConsumer(ILogger<CompanyDeletedConsumer> logger, IMemoryCacheService memoryCacheService)
    {
        _logger = logger;
        _memoryCacheService = memoryCacheService;
    }

    public Task Consume(ConsumeContext<CompanyDeletedEvent> context)
    {
        _logger.LogInformation("Processing cache reset for company {companyId}", context.Message.CompanyId);

        
        
        _logger.LogInformation("Cache reset processed for company {companyId}", context.Message.CompanyId);
        return Task.CompletedTask;
        
    }
}