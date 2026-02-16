namespace QApplication.Responses.ReportResponse;

public class ReviewReportItemResponseModel
{
    public int Id { get; set; }
    public int QueueId { get; set; }
    public string CustomerName { get; set; }
    public string EmployeeName { get; set; }
    public string ServiceName { get; set; }
    public string CompanyName { get; set; }
    public int Grade { get; set; }
    public string? ReviewText { get; set; }
}