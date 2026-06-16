namespace QUserService.Application.Responses;

public class BlockedCustomerResponseModel
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int CustomerId { get; set; }
    
    public string? Reason { get; set; }
    public DateTime BannedUntil { get; set; }
    public bool DoesBanForever { get; set; }
}