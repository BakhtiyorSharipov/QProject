namespace QApplication.Responses.ReportResponse;

public class QueueReportItemResponseModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string EmployeeName { get; set; }
    public string ServiceName { get; set; }
    public string CompanyName { get; set; }
    public string Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
}