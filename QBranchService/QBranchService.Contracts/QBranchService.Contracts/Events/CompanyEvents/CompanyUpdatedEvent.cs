namespace QBranchService.Contracts.Events.CompanyEvents;

public class CompanyUpdatedEvent
{
    public DateTimeOffset OccuredAt { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
}