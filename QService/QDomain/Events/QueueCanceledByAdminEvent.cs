using MediatR;

namespace QDomain.Events;

public class QueueCanceledByAdminEvent: BaseEvent, INotification
{
    public string? Reason { get; set; }
}