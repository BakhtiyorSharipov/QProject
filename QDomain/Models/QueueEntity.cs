using System.ComponentModel.DataAnnotations.Schema;
using QDomain.Enums;
using QDomain.Events;

namespace QDomain.Models;

public class QueueEntity: BaseEntity
{
    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }
    public string? CancelReason { get; set; }

    public QueueStatus Status { get; set; } = QueueStatus.Pending;
    public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
    
    public bool IsStartingSoonNotified { get; set; } = false;

    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; }
    
    public int CustomerId { get; set; }
    public CustomerEntity Customer { get; set; }
    
    public int ServiceId { get; set; }
    public ServiceEntity Service { get; set; }
    
    
}