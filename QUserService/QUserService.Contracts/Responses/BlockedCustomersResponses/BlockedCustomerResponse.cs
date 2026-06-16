using MessagePack;

namespace QUserService.Contracts.Responses.BlockedCustomersResponses;

[MessagePackObject]
public class BlockedCustomerResponse
{
    [Key(0)]
    public int Id { get; set; }
    
    [Key(1)]
    public int CustomerId { get; set; }
    
    [Key(2)]
    public int CompanyId { get; set; }
    
    [Key(3)]
    public string? Reason { get; set; }
    
    [Key(4)]
    public DateTime BannedUntil { get; set; }
    
    [Key(5)]
    public bool DoesBanForever { get; set; }
    
    [Key(6)]
    public DateTime CreatedAt { get; set; }
    
    [Key(7)]
    public bool IsValid { get; set; }
    
    [Key(8)]
    public string? ErrorMessage { get; set; }
}







