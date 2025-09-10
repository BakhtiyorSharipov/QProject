namespace QDomain.Models;

public class CompanyEntity: BaseEntity
{
    public string CompanyName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }

    public List<ServiceEntity> Services { get; set; } = new();

}