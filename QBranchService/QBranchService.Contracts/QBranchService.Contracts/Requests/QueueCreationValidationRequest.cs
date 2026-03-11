using MessagePack;

namespace QBranchService.Contracts.Requests;

[MessagePackObject]
public class QueueCreationValidationRequest
{
    [Key(0)]
    public Guid RequestId { get; set; }
    
    [Key(1)]
    public int BranchId { get; set; }
    
    [Key(2)]
    public DateTimeOffset RequestedStartTime { get; set; }  
}