namespace QBranchService.Domain.Models;

public class BranchEntity
{
    public int Id { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public int CompanyId { get; set; }
    public CompanyEntity Company { get; set; }
}