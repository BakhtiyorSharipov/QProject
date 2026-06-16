
using MessagePack;

namespace QUserService.Contracts.Requests.UserRequests;

[MessagePackObject]
public class UserByIdRequest
{
    [Key(0)]
    public int UserId { get; set; }
    
    [Key(1)]
    public Guid RequestId { get; set; }
}







