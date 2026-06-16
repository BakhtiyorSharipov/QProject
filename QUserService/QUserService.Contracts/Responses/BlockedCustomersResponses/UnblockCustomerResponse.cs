using MessagePack;

namespace QUserService.Contracts.Responses.BlockedCustomersResponses;

[MessagePackObject]
public class UnblockCustomerResponse
{
    [Key(0)]
    public bool Success { get; set; }
    
    [Key(1)]
    public string? ErrorMessage { get; set; }
}