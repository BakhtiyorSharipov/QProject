using MediatR;

namespace QContracts.Events;

public class QueueCanceledByEmployeeEvent: BaseEvent
{
    public string? Reason { get; set; }
}