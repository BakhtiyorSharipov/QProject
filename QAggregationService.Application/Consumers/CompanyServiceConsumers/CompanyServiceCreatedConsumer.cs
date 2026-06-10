using MassTransit;
using Microsoft.Extensions.Logging;
using QAggregationService.Application.Caching;
using QBranchService.Contracts.Events.CompanyServiceEvents;

namespace QAggregationService.Application.Consumers.CompanyServiceConsumers;

public class CompanyServiceCreatedConsumer : IConsumer<CompanyServiceCreatedEvent>
{
    private readonly ILogger<CompanyServiceCreatedConsumer> _logger;
    private readonly IMemoryCacheService _memoryCacheService;

    public CompanyServiceCreatedConsumer(ILogger<CompanyServiceCreatedConsumer> logger,
        IMemoryCacheService memoryCacheService)
    {
        _logger = logger;
        _memoryCacheService = memoryCacheService;
    }

    public Task Consume(ConsumeContext<CompanyServiceCreatedEvent> context)
    {
        _logger.LogInformation("Processing cache reset for CompanyServices {companyId}", context.Message.CompanyId);

        _memoryCacheService.Remove(CacheKeys.CompanyServiceId(context.Message.CompanyServiceId));
        _memoryCacheService.Remove(CacheKeys.CompanyServices(context.Message.CompanyId));

        _logger.LogInformation("Cache reset processed for CompanyServices {companyId}", context.Message.CompanyId);
        return Task.CompletedTask;
    }
}