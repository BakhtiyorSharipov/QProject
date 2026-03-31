using MessagePack;

namespace QAggregationService.Contracts.Responses;

[MessagePackObject]
public class CompanyReportResponse
{
    [Key(0)] public int CompanyId { get; set; }
    [Key(1)] public string CompanyName { get; set; }
    [Key(2)] public DateTime ReportDate { get; set; }
    
    [Key(5)] public int TotalQueues { get; set; }
    [Key(6)] public int CompletedQueues { get; set; }
    [Key(7)] public int PendingQueues { get; set; }
    [Key(8)] public int ConfirmedQueues { get; set; }
    [Key(9)] public int CancelledQueues { get; set; }
    [Key(10)] public int DidNotComeQueues { get; set; }
    
    [Key(13)] public double AverageRating { get; set; }
    [Key(14)] public int TotalReviews { get; set; }
    [Key(15)] public int FiveStarReviews { get; set; }
    [Key(16)] public int FourStarReviews { get; set; }
    [Key(17)] public int ThreeStarReviews { get; set; }
    [Key(18)] public int TwoStarReviews { get; set; }
    [Key(19)] public int OneStarReviews { get; set; }
    
    [Key(20)] public int TotalComplaints { get; set; }
    [Key(21)] public int PendingComplaints { get; set; }
    [Key(22)] public int ReviewedComplaints { get; set; }
    [Key(23)] public int ResolvedComplaints { get; set; }

    
    [Key(24)] public int TotalEmployees { get; set; }
    [Key(25)] public int TotalCustomers { get; set; }
    [Key(26)] public int TotalBlockedCustomers { get; set; }
    
    [Key(27)] public int PageNumber { get; set; }
    [Key(28)] public int PageSize { get; set; }
    [Key(29)] public int TotalRecords { get; set; }
    [Key(30)] public int TotalPages { get; set; }
    
    [Key(31)] public List<QueueReportItem>? Queues { get; set; }
    
    
}





