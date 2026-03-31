using MessagePack;

namespace QAggregationService.Contracts.Responses;

[MessagePackObject]
public class EmployeeReportItem
{
    [Key(0)] public int EmployeeId { get; set; }
    [Key(1)] public string EmployeeName { get; set; }
    [Key(2)] public int BranchId { get; set; }
    [Key(3)] public string BranchName { get; set; }
    [Key(4)] public int TotalQueues { get; set; }
    [Key(5)] public int CompletedQueues { get; set; }
    [Key(6)] public int PendingQueues { get; set; }
    [Key(7)] public int ConfirmedQueues { get; set; }
    [Key(8)] public int CancelledQueues { get; set; }
    [Key(9)] public int DidNotComeQueues { get; set; }
    [Key(11)] public double AverageRating { get; set; }
}