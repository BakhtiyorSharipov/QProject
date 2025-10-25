namespace QApplication.Requests.ReportRequest;

public class EmployeeReportRequest
{ 
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? EmployeeId { get; set; }
    public int? CompanyId { get; set; }
    public int? ServiceId { get; set; }

}