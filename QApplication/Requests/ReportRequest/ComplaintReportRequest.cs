using QDomain.Enums;

namespace QApplication.Requests.ReportRequest;

public class ComplaintReportRequest
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? EmployeeId { get; set; }
    public int? ServiceId { get; set; }
    public int? CompanyId { get; set; }
    public ComplaintStatus? Status { get; set; }
    public bool IncludeStatistics { get; set; } = true;
}