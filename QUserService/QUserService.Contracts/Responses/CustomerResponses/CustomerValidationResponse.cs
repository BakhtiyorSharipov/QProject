using MessagePack;

namespace QUserService.Contracts.Responses.CustomerResponses;

[MessagePackObject]
public class CustomerValidationResponse
{
    [Key(0)]
    public bool IsValid { get; set; }
    
    [Key(1)]
    public bool IsBlocked { get; set; }
    
    [Key(2)]
    public string? BlockReason { get; set; }
    
    [Key(3)]
    public DateTime? BannedUntil { get; set; }
    
    [Key(4)]
    public string? ErrorMessage { get; set; }
}