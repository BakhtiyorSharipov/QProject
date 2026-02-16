namespace QDomain.Models;

public class CustomerEntity: BaseEntity 
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ReviewEntity> Reviews { get; set; } = new();

    public List<QueueEntity> Queues { get; set; } = new();
    public List<ComplaintEntity> Complaints { get; set; } = new();
}