namespace QApplication.Responses.ReportResponse;

public class QueueReportResponseModel
{
    public int TotalQueues { get; set; }
    public int Completed { get; set; }
    public int Pending { get; set; }
    public int CancelledByCustomer { get; set; }
    public int CancelledByEmployee { get; set; }
    public int DidNotCome { get; set; }
}