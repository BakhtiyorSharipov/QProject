namespace QBranchService.Domain.Models;

public class CompanyServiceEntity
{
    public int Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceDescription { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public int CompanyId { get; set; }

    public required CompanyEntity Company { get; set; }
    // public List<EmployeeEntity> Employees { get; set; } = [];
}