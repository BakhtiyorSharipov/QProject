using MediatR;

namespace QDomain.Events;

public class QueueCanceledByEmployeeEvent: BaseEvent, INotification
{
    public string? Reason { get; set; }
}