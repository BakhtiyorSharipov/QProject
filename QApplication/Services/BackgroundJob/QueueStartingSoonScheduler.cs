using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QDomain.Enums;
using QDomain.Events;

namespace QApplication.Services.BackgroundJob;

public class QueueStartingSoonScheduler: BackgroundService
{
    private readonly ILogger<QueueStartingSoonScheduler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public QueueStartingSoonScheduler(ILogger<QueueStartingSoonScheduler> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow;
            var fiveMinuteLater = now.AddMinutes(2);
            
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider
                .GetRequiredService<IQueueApplicationDbContext>();

            var publishEndpoint = scope.ServiceProvider
                .GetRequiredService<IPublishEndpoint>();

            var queuesStartingSoon = await dbContext.Queues
                .Where(q => q.Status == QueueStatus.Confirmed)
                .Where(q => q.StartTime >= now && q.StartTime <= fiveMinuteLater)
                .ToListAsync(stoppingToken);

            foreach (var queue in queuesStartingSoon)
            {
                var eventMessage = new QueueStartingSoonEvent
                {
                    QueueId = queue.Id,
                    CustomerId = queue.CustomerId,
                    EmployeeId = queue.EmployeeId,
                    StartTime = queue.StartTime
                };

                await publishEndpoint.Publish(eventMessage, stoppingToken);
                _logger.LogInformation("Published QueueStartingSoonEvent for QueueId {QueueId}", queue.Id);
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
        
    }
}