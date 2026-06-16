using MessagePack;

namespace QUserService.Contracts.Requests.EmployeeRequests;

[MessagePackObject]
public class GetEmployeeByUserIdRequest
{
    [Key(0)]
    public int UserId { get; set; }
    
    [Key(1)]
    public Guid RequestId { get; set; }
}