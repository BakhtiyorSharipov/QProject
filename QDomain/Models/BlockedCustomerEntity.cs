namespace QDomain.Models;

public class BlockedCustomerEntity: BaseEntity  
{
    public string? Reason { get; set; }
    public DateTime BannedUntil { get; set; }
    public bool DoesBanForever { get; set; }
    
    public int CompanyId { get; set; }
    public CompanyEntity Company { get; set; }
    
    public int CustomerId { get; set; }
    public CustomerEntity Customer { get; set; }
    
}