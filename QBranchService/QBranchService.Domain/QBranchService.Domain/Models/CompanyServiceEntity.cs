namespace QBranchService.Domain.Models;

public class CompanyServiceEntity
{
    public int Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceDescription { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } 

    public int CompanyId { get; set; }

    public CompanyEntity Company { get; set; }
}