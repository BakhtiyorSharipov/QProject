using MessagePack;

namespace QAggregationService.Contracts.Requests;

[MessagePackObject]
public class EmployeeReportRequest
{
    [Key(0)] public int EmployeeId { get; set; }
    [Key(1)] public int? CompanyId { get; set; } 
    [Key(2)] public DateTime? FromDate { get; set; } 
    [Key(3)] public DateTime? ToDate { get; set; }
}