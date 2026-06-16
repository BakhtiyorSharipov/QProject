
using MessagePack;

namespace QUserService.Contracts.Requests.CustomerRequests;

[MessagePackObject]
public class CustomerByIdRequest
{
    [Key(0)]
    public int CustomerId { get; set; }
    
    [Key(1)]
    public Guid RequestId { get; set; }
}







