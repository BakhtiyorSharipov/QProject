using MessagePack;

namespace QAggregationService.Contracts.Responses;

[MessagePackObject]
public class ServiceReportItem
{
    [Key(0)] public int ServiceId { get; set; }
    [Key(1)] public string ServiceName { get; set; }
    [Key(2)] public int TotalQueues { get; set; }
    [Key(3)] public int CompletedQueues { get; set; }
    [Key(4)] public int PendingQueues { get; set; }
    [Key(5)] public int ConfirmedQueues { get; set; }
    [Key(6)] public int CancelledQueues { get; set; }
    [Key(7)] public int DidNotComeQueues { get; set; }
    [Key(8)] public double AverageDurationMinutes { get; set; }
    [Key(9)] public double AverageRating { get; set; }
}