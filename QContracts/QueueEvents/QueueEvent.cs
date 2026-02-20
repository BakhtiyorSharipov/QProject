using QContracts.QueueEvents.Enums;

namespace QContracts.QueueEvents;

public class QueueEvent
{
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    public int QueueId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public QueueEventType EventType { get; set; }
    public UpdatedQueueStatus? Status { get; set; } 
    public string? CancelReason { get; set; }   
}