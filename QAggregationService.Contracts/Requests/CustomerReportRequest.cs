using MessagePack;

namespace QAggregationService.Contracts.Requests;

[MessagePackObject]
public class CustomerReportRequest
{
    [Key(0)] public int CustomerId { get; set; }
    [Key(1)] public DateTime? FromDate { get; set; } 
    [Key(2)] public DateTime? ToDate { get; set; }
}