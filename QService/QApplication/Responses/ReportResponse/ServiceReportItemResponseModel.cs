namespace QApplication.Responses.ReportResponse;

public class ServiceReportItemResponseModel
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; }
    public string CompanyName { get; set; }
    public int TotalQueues { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public int DidNotCount { get; set; }
    public int PendingCount { get; set; }
    public int ConfirmedCount { get; set; }
}