
namespace QContracts.NotificationEvents;

public class SendNotificationEvent
{
    public int UserId { get; set; }
    public string Message { get; set; }
}