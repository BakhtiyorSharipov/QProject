using MessagePack;

namespace QUserService.Contracts.Responses.BlockedCustomersResponses;

[MessagePackObject]
public class BlockCustomerResponse
{
    [Key(0)]
    public bool Success { get; set; }
    
    [Key(1)]
    public int BlockedCustomerId { get; set; }
    
    [Key(2)]
    public string? ErrorMessage { get; set; }
}
