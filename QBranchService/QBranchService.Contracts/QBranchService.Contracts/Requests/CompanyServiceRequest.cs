namespace QBranchService.Contracts.Requests;

public class CompanyServiceRequest
{
    public Guid RequestId { get; set; }
    public int CompanyServiceId { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
}