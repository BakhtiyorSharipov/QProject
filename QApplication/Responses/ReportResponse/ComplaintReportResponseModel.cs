namespace QApplication.Responses.ReportResponse;

public class ComplaintReportResponseModel
{
    public List<ComplaintReportItemResponseModel> Complaints { get; set; } = new();
    
    public int PendingCount { get; set; }
    public int ReviewedCount { get; set; }
    public int ResolvedCount { get; set; }
    public int TotalComplaints { get; set; }
}