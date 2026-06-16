
using MessagePack;

namespace QUserService.Contracts.Requests.EmployeeRequests;

[MessagePackObject]
public class EmployeeByIdRequest
{
    [Key(0)]
    public int EmployeeId { get; set; }
    
    [Key(1)]
    public Guid RequestId { get; set; }
}







