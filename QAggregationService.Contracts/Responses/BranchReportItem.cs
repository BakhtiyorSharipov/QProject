using MessagePack;

namespace QAggregationService.Contracts.Responses;

[MessagePackObject]
public class BranchReportItem
{
    [Key(0)] public int BranchId { get; set; }
    [Key(1)] public string BranchName { get; set; }
    [Key(2)] public string Address { get; set; }
    [Key(3)] public int TotalQueues { get; set; }
    [Key(4)] public int CompletedQueues { get; set; }
    [Key(5)] public int PendingQueues { get; set; }
    [Key(6)] public int ConfirmedQueues { get; set; }
    [Key(7)] public int CancelledQueues { get; set; }
    [Key(8)] public int DidNotComeQueues { get; set; }
    [Key(9)] public double AverageWaitMinutes { get; set; }
    [Key(11)] public double AverageRating { get; set; }
    [Key(12)] public int TotalEmployees { get; set; }
    [Key(13)] public int MaxTicketsPerDay { get; set; }
    [Key(14)] public TimeOnly OpenTime { get; set; }
    [Key(15)] public TimeOnly CloseTime { get; set; }
    [Key(16)] public TimeOnly? BreakStartTime { get; set; }
    [Key(17)] public TimeOnly? BreakEndTime { get; set; }
}