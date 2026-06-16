using MessagePack;

namespace QUserService.Contracts.Requests.UserRequests;

[MessagePackObject]
public class GetUserByEmployeeIdRequest
{
    [Key(0)]
    public int EmployeeId { get; set; }
    
    [Key(1)]
    public Guid RequestId { get; set; }
}