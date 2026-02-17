using QContracts.NotificationEvents.Enums;

namespace QContracts.NotificationEvents;

public class SendNotificationEvent
{
    public int UserId { get; set; }
    public NotificationTitle Title { get; set; }
    public string Message { get; set; }
}