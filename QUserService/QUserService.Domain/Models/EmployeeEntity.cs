namespace QUserService.Domain.Models;

public class EmployeeEntity : BaseEntity
{
    public int CompanyId { get; set; }
    public int? BranchId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Position { get; set; }
    public string PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; }
    public int? ServiceId { get; set; }


    public List<AvailabilityScheduleEntity> AvailabilitySchedules { get; set; }
}