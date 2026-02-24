namespace QBranchService.Contracts.Requests;

public class CompanyRequest
{
    public Guid RequestId { get; set; }
    public int CompanyId { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
}