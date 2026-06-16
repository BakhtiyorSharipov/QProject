using MessagePack;

namespace QUserService.Contracts.Responses.BlockedCustomersResponses;

[MessagePackObject]
public class BlockedCustomerValidationResponse
{
    [Key(0)]
    public bool IsBlocked { get; set; }
    
    [Key(1)]
    public bool IsBlockedForever { get; set; }
    
    [Key(2)]
    public DateTime? BannedUntil { get; set; }
    
    [Key(3)]
    public string? BlockReason { get; set; }
    
    [Key(4)]
    public string? ErrorMessage { get; set; }
}