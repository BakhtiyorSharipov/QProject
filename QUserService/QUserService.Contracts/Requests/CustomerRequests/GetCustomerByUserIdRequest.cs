using MessagePack;

namespace QUserService.Contracts.Requests.CustomerRequests;

[MessagePackObject]
public class GetCustomerByUserIdRequest
{
    [Key(0)]
    public int UserId { get; set; }
    
    [Key(1)]
    public Guid RequestId { get; set; }
}