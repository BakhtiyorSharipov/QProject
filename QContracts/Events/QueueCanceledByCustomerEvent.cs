using MediatR;

namespace QContracts.Events;

public class QueueCanceledByCustomerEvent: BaseEvent
{
    public string? Reason { get; set; }
}