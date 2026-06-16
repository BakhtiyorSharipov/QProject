using MessagePack;

namespace QUserService.Contracts.Requests.UserRequests;

[MessagePackObject]
public class GetUserByCustomerIdRequest
{
    [Key(0)]
    public int CustomerId { get; set; }
    
    [Key(1)]
    public Guid RequestId { get; set; }
}