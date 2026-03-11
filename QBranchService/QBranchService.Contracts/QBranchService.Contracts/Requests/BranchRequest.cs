using MessagePack;

namespace QBranchService.Contracts.Requests;

[MessagePackObject]
public class BranchRequest
{
    [Key(0)]public Guid RequestId { get; set; }
    [Key(1)] public int CompanyId { get; set; }
    [Key(2)]public int BranchId { get; set; }
    [Key(3)]public DateTimeOffset RequestedAt { get; set; }
}