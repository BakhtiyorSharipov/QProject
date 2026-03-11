using MessagePack;

namespace QBranchService.Contracts.Responses;

[MessagePackObject]
public class BranchConfigurationResponse
{
   [Key(0)] public Guid RequestId { get; set; }
   [Key(1)] public int BranchId { get; set; }
   [Key(2)] public bool IsOpen { get; set; }
   [Key(3)] public int MaxTickets { get; set; }
   [Key(6)] public string? ErrorMessage { get; set; }
}