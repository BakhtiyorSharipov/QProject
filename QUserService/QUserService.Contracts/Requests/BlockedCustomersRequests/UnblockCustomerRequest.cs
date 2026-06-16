using MessagePack;

namespace QUserService.Contracts.Requests.BlockedCustomersRequests;

[MessagePackObject]
public class UnblockCustomerRequest
{
    [Key(0)]
    public int BlockedCustomerId { get; set; }
    
    [Key(1)]
    public int UnblockedByUserId { get; set; }
    
    [Key(2)]
    public Guid RequestId { get; set; }
}