namespace QApplication.Responses.ReportResponse;

public class EmployeeReportItemResponseModel
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string CompanyName { get; set; }
    public string ServiceName { get; set; }
    public int TotalQueues { get; set; }
    public int CompletedQueues { get; set; }
    
    public int PendingQueues { get; set; }
    public int ConfirmedQueues { get; set; }
    public int CancelledQueues { get; set; }
    public int DidNotComeQueues { get; set; }
}