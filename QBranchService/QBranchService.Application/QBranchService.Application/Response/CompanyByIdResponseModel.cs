namespace QBranchService.Application.Response;

public class CompanyByIdResponseModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int TotalBranches { get; set; }
    public int TotalServices { get; set; }
    public List<CompanyBranchesResponseModel> Branches { get; set; } = [];
    public List<CompanyServiceResponseModel> Services { get; set; } = [];
}