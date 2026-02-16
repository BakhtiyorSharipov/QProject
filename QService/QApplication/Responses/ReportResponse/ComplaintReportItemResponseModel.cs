namespace QApplication.Responses.ReportResponse;

public class ComplaintReportItemResponseModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string EmployeeName { get; set; }
    public string ServiceName { get; set; }
    public string CompanyName { get; set; }
    public string ComplaintText { get; set; }
    public string ResponseText { get; set; }
    public string Status { get; set; }
}