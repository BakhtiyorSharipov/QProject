
namespace QContracts.SmsEvents;

public class QueueCanceledByAdminEvent: BaseEvent
{
    public string? Reason { get; set; }
}