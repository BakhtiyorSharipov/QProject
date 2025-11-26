namespace QApplication.Responses.ReportResponse;

public class QueueReportResponseModel
{
    public List<QueueReportItemResponseModel> Queues { get; set; } = new();
    
    public int TotalQueues { get; set; }
    public int CompletedCount { get; set; }
    public int PendingCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int CanceledCount { get; set; }
    public int DidNotComeCount { get; set; }
}