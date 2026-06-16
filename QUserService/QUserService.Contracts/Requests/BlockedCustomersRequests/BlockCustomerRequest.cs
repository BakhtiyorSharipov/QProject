using MessagePack;

namespace QUserService.Contracts.Requests.BlockedCustomersRequests;

[MessagePackObject]
public class BlockCustomerRequest
{
    [Key(0)]
    public int CustomerId { get; set; }
    
    [Key(1)]
    public int CompanyId { get; set; }
    
    [Key(2)]
    public string? Reason { get; set; }
    
    [Key(3)]
    public DateTime BannedUntil { get; set; }
    
    [Key(4)]
    public bool DoesBanForever { get; set; }
    
    [Key(5)]
    public int? BlockedByUserId { get; set; }
    
    [Key(6)]
    public Guid RequestId { get; set; }
}