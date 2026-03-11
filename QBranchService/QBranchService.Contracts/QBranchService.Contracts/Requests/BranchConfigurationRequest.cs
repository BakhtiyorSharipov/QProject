using MessagePack;

namespace QBranchService.Contracts.Requests;

[MessagePackObject]
public class BranchConfigurationRequest
{
    [Key(0)]public Guid RequestId { get; set; }
    [Key(1)]public int BranchId { get; set;  }
    [Key(2)]public DateTimeOffset StartTime { get; set; }
    [Key(3)]public int ServiceId { get; set; }
}