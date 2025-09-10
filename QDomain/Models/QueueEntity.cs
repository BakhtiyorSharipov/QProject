namespace QDomain.Models;

public class QueueEntity: BaseEntity
{
    public string DayOfWeek { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? CancelReason { get; set; }
    
    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; }
    
    public int CustomerId { get; set; }
    public CustomerEntity Customer { get; set; }
    
    public int ServiceId { get; set; }
    public ServiceEntity Service { get; set; }
    
}