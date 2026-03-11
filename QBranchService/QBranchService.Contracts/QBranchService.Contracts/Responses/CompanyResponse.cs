
namespace QBranchService.Contracts.Responses;

public class CompanyResponse
{
    public Guid RequestId { get; set; }
    public int CompanyId { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CompanyName { get; set; }
}