namespace QBranchService.Domain.Models;

public class CompanyEntity
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public List<CompanyServiceEntity> CompanyServices { get; set; }
    public List<BranchEntity> Branches { get; set; }
}