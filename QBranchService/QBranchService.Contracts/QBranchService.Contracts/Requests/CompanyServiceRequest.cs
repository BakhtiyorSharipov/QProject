using MessagePack;

namespace QBranchService.Contracts.Requests;

[MessagePackObject]
public class CompanyServiceRequest
{
    [Key(0)] public Guid RequestId { get; set; }
    [Key(1)] public int CompanyId { get; set; }
    [Key(2)] public int CompanyServiceId { get; set; }
    [Key(3)] public DateTimeOffset RequestedAt { get; set; }
}