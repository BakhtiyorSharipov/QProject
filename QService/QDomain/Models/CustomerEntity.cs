namespace QDomain.Models;

public class CustomerEntity: BaseEntity 
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }

    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public List<ReviewEntity> Reviews { get; set; } = new();

    public List<QueueEntity> Queues { get; set; } = new();
    public List<ComplaintEntity> Complaints { get; set; } = new();
}