using MessagePack;

namespace QAggregationService.Contracts.Responses;

[MessagePackObject]
public class DashboardResponse
{
    [Key(0)] public int CompanyId { get; set; }
    [Key(1)] public string CompanyName { get; set; }
    [Key(2)] public DateTime ReportDate { get; set; }

    [Key(5)] public int TotalBranches { get; set; }
    [Key(6)] public int TotalServices { get; set; }
    [Key(7)] public int TotalCustomers { get; set; }
    [Key(8)] public int TotalBlockedCustomers { get; set; }
    [Key(9)] public int TotalEmployees { get; set; }
    [Key(10)] public int TotalQueues { get; set; }
    [Key(11)] public int CompletedQueues { get; set; }
    [Key(12)] public int PendingQueues { get; set; }
    [Key(13)] public int CancelledQueues { get; set; }
    [Key(14)] public int DidNotComeQueues { get; set; }
    [Key(15)] public int TotalReviews { get; set; }
    [Key(16)] public int TotalComplaints { get; set; }
    [Key(17)] public int TotalPendingComplaints { get; set; }
    [Key(18)] public int TotalReviewedComplaints { get; set; }
    [Key(19)] public int TotalResolvedComplaints { get; set; }
    
    [Key(20)] public string? TopServiceName { get; set; }
    [Key(21)] public int? TopServiceQueueCount { get; set; }
    [Key(22)] public string? TopBranchName { get; set; }
    [Key(23)] public int? TopBranchQueueCount { get; set; }
    [Key(24)] public string? TopEmployeeName { get; set; }
    [Key(25)] public int? TopEmployeeQueueCount { get; set; }
    [Key(26)] public string? TopCustomerName { get; set; }
    [Key(27)] public int? TopCustomerQueueCount { get; set; }
    

    [Key(30)] public List<QueueReportItem>? RecentQueues { get; set; }
}