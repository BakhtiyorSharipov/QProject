using MessagePack;

namespace QAggregationService.Contracts.Responses;

[MessagePackObject]
public class EmployeeReportResponse
{
    [Key(0)] public int EmployeeId { get; set; }
    [Key(1)] public string EmployeeName { get; set; }
    [Key(2)] public int TotalQueues { get; set; }
    [Key(3)] public int CompletedQueues { get; set; }
    [Key(4)] public int PendingQueues { get; set; }
    [Key(5)] public int CancelledQueues { get; set; }
    [Key(6)] public int DidNotComeQueues { get; set; }
    [Key(7)] public double AverageReviewGrade { get; set; }
    [Key(8)] public int TotalReviewsReceived { get; set; }
    [Key(9)] public int TotalComplaintsReceived { get; set; }
    [Key(10)] public int PendingComplaints { get; set; }
    [Key(11)] public int ReviewedComplaints { get; set; }
    [Key(12)] public int ResolvedComplaints { get; set; }
    [Key(13)] public List<QueueReportItem>? RecentQueues { get; set; } 
}