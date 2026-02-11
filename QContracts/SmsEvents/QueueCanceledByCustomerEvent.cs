namespace QContracts.SmsEvents;

public class QueueCanceledByCustomerEvent: BaseEvent
{
    public string? Reason { get; set; }
}