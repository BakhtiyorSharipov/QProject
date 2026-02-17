using MassTransit;
using QContracts.NotificationEvents;
using QNotificationService.Application.Interfaces;

namespace QNotificationService.Infrastructure.Consumers;

public class SendNotificationConsumer : IConsumer<SendNotificationEvent>
{
    private readonly INotificationService _notificationService;

    public SendNotificationConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<SendNotificationEvent> context)
    {
        await _notificationService.SendAsync(context.Message.UserId, context.Message.Message,
            context.CancellationToken);
    }
}