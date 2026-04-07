namespace QBranchService.Contracts.Events.CompanyServiceEvents;

public class CompanyServiceCreatedEvent
{
    public DateTimeOffset OccuredAt { get; set; }
    public int CompanyId { get; set; }
    public int CompanyServiceId { get; set; }
    public string ServiceName { get; set; }
    public string ServiceDescription { get; set; }
}