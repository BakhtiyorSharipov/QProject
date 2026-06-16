
using MessagePack;

namespace QUserService.Contracts.Requests.BlockedCustomersRequests;

[MessagePackObject]
public class BlockedCustomerByIdRequest
{
    [Key(0)]
    public int BlockedCustomerId { get; set; }
    
    [Key(1)]
    public Guid RequestId { get; set; }
}
