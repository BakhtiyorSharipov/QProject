using MessagePack;

namespace QUserService.Contracts.Responses.BlockedCustomersResponses;

[MessagePackObject]
public class BlockedCustomerInfo
{
    [Key(1)] public int BlockedId { get; set; }
    [Key(2)] public int CustomerId { get; set; }
    [Key(3)] public int CompanyId { get; set; }
    [Key(4)] public string CustomerName { get; set; }
    [Key(5)] public string? Reason { get; set; }
    [Key(6)] public DateTime BannedUntil { get; set; }
    [Key(7)] public bool DoesBanForever { get; set; }
    [Key(8)] public DateTime CreatedAt { get; set; }
}