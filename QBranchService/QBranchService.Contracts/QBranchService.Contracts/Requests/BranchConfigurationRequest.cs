namespace QBranchService.Contracts.Requests;

public class BranchConfigurationRequest
{
    public Guid RequestId { get; set; }
    public int BranchId { get; set;  }
    public DateTimeOffset StartTime { get; set; }
    public int ServiceId { get; set; }
}