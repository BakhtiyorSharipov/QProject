using MediatR;

namespace QContracts.Events;

public class QueueCanceledByAdminEvent: BaseEvent
{
    public string? Reason { get; set; }
}