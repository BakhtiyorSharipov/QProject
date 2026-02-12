namespace QContracts.SmsEvents;

public class QueueCanceledByEmployeeEvent: BaseEvent
{
    public string? Reason { get; set; }
}