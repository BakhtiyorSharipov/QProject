using MessagePack;

namespace QAggregationService.Contracts.Responses;

[MessagePackObject]
public class CustomerReportResponse
{
    [Key(0)] public int TotalCustomers { get; set; }
    [Key(1)] public int BlockedCustomers { get; set; }
    [Key(2)] public int DidNotComeCustomers { get; set; }
    
    [Key(3)] public List<CustomerReportItem>? TopCustomers { get; set; }
}