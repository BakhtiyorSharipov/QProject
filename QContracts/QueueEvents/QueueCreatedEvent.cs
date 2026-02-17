namespace QContracts.QueueEvents;

public class QueueCreatedEvent
{
    public DateTimeOffset OccurredAt { get; set; }
    public int QueueId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public DateTimeOffset StartTime { get; set; }
}