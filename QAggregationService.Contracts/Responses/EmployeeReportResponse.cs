using MessagePack;

namespace QAggregationService.Contracts.Responses;

[MessagePackObject]
public class EmployeeReportResponse
{
    [Key(0)] public int EmployeeId { get; set; }
    [Key(1)] public string EmployeeName { get; set; }
    [Key(2)] public int TotalQueuesHandled { get; set; }
    [Key(3)] public int CompletedQueues { get; set; }
    [Key(4)] public int PendingQueues { get; set; }
    [Key(5)] public int CancelledQueues { get; set; }
    [Key(6)] public int DidNotComeQueues { get; set; }
    [Key(7)] public double AverageReviewGrade { get; set; }
    [Key(8)] public int TotalReviewsReceived { get; set; }
    [Key(9)] public int TotalComplaintsReceived { get; set; }
    [Key(10)] public int ResolvedComplaints { get; set; }
    [Key(11)] public List<QueueReportItem>? RecentQueues { get; set; } 
}