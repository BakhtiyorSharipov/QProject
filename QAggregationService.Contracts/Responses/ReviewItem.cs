using MessagePack;

namespace QAggregationService.Contracts.Responses;

[MessagePackObject]
public class ReviewItem
{
    [Key(0)] public int ReviewId { get; set; }
    [Key(1)] public int QueueId { get; set; }
    [Key(2)] public int CustomerId { get; set; }
    [Key(3)] public string CustomerName { get; set; }
    [Key(4)] public int? EmployeeId { get; set; }
    [Key(5)] public string? EmployeeName { get; set; }
    [Key(6)] public int BranchId { get; set; }
    [Key(7)] public string BranchName { get; set; }
    [Key(8)] public int ServiceId { get; set; }
    [Key(9)] public string ServiceName { get; set; }
    [Key(10)] public int Grade { get; set; }
    [Key(11)] public string? ReviewText { get; set; }
}