namespace QDomain.Models;

public class ReviewEntity: BaseEntity   
{
    public int Grade { get; set; }
    public string? ReviewText { get; set; }
    
    public int EmployeeId { get; set; }
    public EmployeeEntity Employee { get; set; }
    
    public int QueueId { get; set; }
    public QueueEntity Queue { get; set; }

    public int CustomerId { get; set; }
    public CustomerEntity Customer { get; set; }
    
}