namespace QApplication.Responses.ReportResponse;

public class CompanyReportItemResponseModel
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    
    public int TotalQueues { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public int DidNotCome { get; set; }
    
    public double? AverageRating { get; set; }
    public int ComplaintCount { get; set; }
    public int EmployeeCount { get; set; }
    public int ServiceCount { get; set; }
    
    public int TotalCustomers { get; set; }
    public int BlockedCustomers { get; set; }
    public List<string> MostPopularServices { get; set; } = new();
}