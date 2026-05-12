
namespace QNotificationService.Contracts.NotificationEvents;

public class SendNotificationEvent
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public required string Message { get; set; }
}