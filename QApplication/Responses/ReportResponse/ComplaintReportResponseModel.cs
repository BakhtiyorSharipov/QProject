namespace QApplication.Responses.ReportResponse;

public class ComplaintReportResponseModel
{
    public int TotalComplaints { get; set; }
    public int Pending { get; set; }
    public int Reviewed { get; set; }
    public int Resolved { get; set; }
}