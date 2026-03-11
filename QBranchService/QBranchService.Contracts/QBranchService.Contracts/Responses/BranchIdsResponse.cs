namespace QBranchService.Contracts.Responses;

public class BranchIdsResponse
{
    public Guid RequestId { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
    public int ServiceId { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}