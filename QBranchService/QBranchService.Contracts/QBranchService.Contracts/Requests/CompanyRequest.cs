using MessagePack;

namespace QBranchService.Contracts.Requests;

[MessagePackObject]
public class CompanyRequest
{
    [Key(0)]public Guid RequestId { get; set; }
    [Key(1)]public int CompanyId { get; set; }
    [Key(2)]public DateTimeOffset RequestedAt { get; set; }
}