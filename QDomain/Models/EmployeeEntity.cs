namespace QDomain.Models;

public class EmployeeEntity: BaseEntity
{
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string Position { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }

    public bool IsActive { get; set; } = true;
    
    public int ServiceId { get; set; }
    public ServiceEntity Service { get; set; }

    public List<QueueEntity> Queues { get; set; } = new();
}