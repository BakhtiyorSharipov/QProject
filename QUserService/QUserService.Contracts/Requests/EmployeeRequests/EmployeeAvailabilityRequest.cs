using MessagePack;

namespace QUserService.Contracts.Requests.EmployeeRequests;

[MessagePackObject]
public class EmployeeAvailabilityRequest
{
    [Key(0)]
    public Guid RequestId { get; set; }
    
    [Key(1)]
    public int EmployeeId { get; set; }
    
    [Key(2)]
    public DateTimeOffset StartTime { get; set; }
    
    [Key(3)]
    public DateTimeOffset? EndTime { get; set; }  
    
    [Key(4)]
    public int? DurationMinutes { get; set; }  
    
    [Key(5)]
    public int? ExistingQueueId { get; set; }
}