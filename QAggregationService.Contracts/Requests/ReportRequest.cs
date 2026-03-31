using MessagePack;
using QContracts.Enums;

namespace QAggregationService.Contracts.Requests;

[MessagePackObject]
public class ReportRequest
{
    [Key(0)] public int? CompanyId { get; set; }
    [Key(1)] public int? BranchId { get; set; }
    [Key(4)] public int? ServiceId { get; set; }
    
    
    [Key(5)] public DateTime? FromDate { get; set; }
    [Key(6)] public DateTime? ToDate { get; set; }
    
    
    
    [Key(7)] public CurrentQueueStatus? QueueStatus { get; set; }
    [Key(8)] public CurrentComplaintStatus? ComplaintStatus { get; set; }
    
    
    
    [Key(9)] public int PageNumber { get; set; } = 1;
    [Key(10)] public int PageSize { get; set; } = 50;
    
    
    

}