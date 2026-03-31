using MessagePack;

namespace QAggregationService.Contracts.Responses;

[MessagePackObject]
public class CustomerReportItem
{
    [Key(0)] public int CustomerId { get; set; }
    [Key(1)] public string CustomerName { get; set; }
    [Key(2)] public string PhoneNumber { get; set; }
    [Key(3)] public int TotalQueues { get; set; }
    [Key(4)] public int CompletedQueues { get; set; }
    [Key(5)] public int CancelledQueues { get; set; }
    [Key(6)] public int DidNotComeQueues { get; set; }
    [Key(7)] public double AverageRating { get; set; }
    [Key(8)] public DateTime? LastVisitDate { get; set; }

}