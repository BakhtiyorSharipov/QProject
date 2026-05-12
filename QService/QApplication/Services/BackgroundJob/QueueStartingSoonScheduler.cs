using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QContracts.Events;
using QContracts.QueueEvents;
using QContracts.QueueEvents.Enums;
using QDomain.Enums;

namespace QApplication.Services.BackgroundJob;

public class QueueStartingSoonScheduler : BackgroundService
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
            var fiveMinuteLater = now.AddMinutes(5);

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider
                .GetRequiredService<IQueueApplicationDbContext>();

            var publishEndpoint = scope.ServiceProvider
                .GetRequiredService<IPublishEndpoint>();

            var queuesStartingSoon = await dbContext.Queues
                .Where(q => q.Status == QueueStatus.Confirmed
                            && q.StartTime >= now && q.StartTime <= fiveMinuteLater
                            && !q.IsStartingSoonNotified)
                .ToListAsync(stoppingToken);

            foreach (var queue in queuesStartingSoon)
            {
                var user =await dbContext.Users.FirstOrDefaultAsync(s => s.CustomerId == queue.CustomerId, stoppingToken);
                var userEmail = user?.EmailAddress;
                
                var eventMessage = new QueueEvent
                {
                    Email = userEmail ?? "test@gmail.com",
                    QueueId = queue.Id,
                    CustomerId = queue.CustomerId,
                    EmployeeId = queue.EmployeeId,
                    StartTime = queue.StartTime,
                    EventType = QueueEventType.StartingSoon,
                };

                await publishEndpoint.Publish(eventMessage, stoppingToken);
                _logger.LogInformation("Published QueueStartingSoonEvent for QueueId {QueueId}", queue.Id);

                queue.IsStartingSoonNotified = true;
            }


            await dbContext.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}