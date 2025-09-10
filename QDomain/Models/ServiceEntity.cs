namespace QDomain.Models;

public class ServiceEntity: BaseEntity
{
    public string ServiceName { get; set; }
    public string ServiceDescription { get; set; }
    
    public int CompanyId { get; set; }
    public CompanyEntity Company { get; set; }

    public List<EmployeeEntity> Employees { get; set; } = new();
}