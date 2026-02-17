namespace QContracts.QueueEvents;

public class QueueStartingSoonEvent
{
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    public int QueueId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public DateTimeOffset StartTime { get; set; }
}