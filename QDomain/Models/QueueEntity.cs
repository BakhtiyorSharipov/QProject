using QDomain.Enums;

namespace QDomain.Models;

public class QueueEntity: BaseEntity
{
    public DateTime StartTime { get; set; }
    public string? CancelReason { get; set; }

    public QueueStatus Status { get; set; } = QueueStatus.Pending;
    
    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; }
    
    public int CustomerId { get; set; }
    public CustomerEntity Customer { get; set; }
    
    public int ServiceId { get; set; }
    public ServiceEntity Service { get; set; }
    
}