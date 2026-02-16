
namespace QContracts.NotificationEvents;

public class QueueCanceledByAdminEvent: BaseEvent
{
    public string? Reason { get; set; }
}