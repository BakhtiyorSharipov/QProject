namespace QBranchService.Contracts.Responses;

public class CompanyServiceResponse
{
    public Guid RequestId { get; set; }
    public int CompanyServiceId { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CompanyServiceName { get; set; }
}