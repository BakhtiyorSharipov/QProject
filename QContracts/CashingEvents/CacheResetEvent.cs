namespace QContracts.CashingEvents;

public class CacheResetEvent
{
    public DateTimeOffset OccuredAt { get; set; }
    public int QueueId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    
}