using MediatR;

namespace QDomain.Events;

public class QueueCanceledByCustomerEvent: BaseEvent, INotification
{
    public string? Reason { get; set; }
}