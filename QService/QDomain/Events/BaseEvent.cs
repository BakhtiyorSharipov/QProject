using MediatR;

namespace QDomain.Events;

public abstract class BaseEvent: INotification
{
    public DateTime OccuredAt { get; set; }
    public int QueueId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }

}