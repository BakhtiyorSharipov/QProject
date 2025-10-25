namespace QApplication.Requests.ReportRequest;

public class CompanyReportRequest
{
    public int CompanyId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}