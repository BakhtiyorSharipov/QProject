using MessagePack;

namespace QUserService.Contracts.Requests.EmployeeRequests;

[MessagePackObject]
public class EmployeeScheduleRequest
{
    [Key(0)]
    public int EmployeeId { get; set; }
    
    [Key(1)]
    public DateTimeOffset Date { get; set; }
    
    [Key(2)]
    public Guid RequestId { get; set; }
}