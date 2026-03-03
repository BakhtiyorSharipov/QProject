using MessagePack;

namespace QBranchService.Contracts.Responses;

[MessagePackObject]
public class BranchResponse
{
    [Key(0)] public Guid RequestId { get; set; }
    [Key(1)] public int CompanyId { get; set; }
    [Key(2)] public int BranchId { get; set; }
    [Key(3)] public bool IsValid { get; set; }
    [Key(4)] public string? ErrorMessage { get; set; }
    [Key(5)] public string? BranchName { get; set; }
}