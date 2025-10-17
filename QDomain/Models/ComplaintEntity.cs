using QDomain.Enums;

namespace QDomain.Models;

public class ComplaintEntity: BaseEntity
{
    public string ComplaintText { get; set; }
    public string? ResponseText { get; set; }
    public ComplaintStatus ComplaintStatus { get; set; } = ComplaintStatus.Pending;
    
    public int CustomerId { get; set; }
    public CustomerEntity Customer { get; set; }
    
    public int QueueId { get; set; }
    public QueueEntity Queue { get; set; }
    
}