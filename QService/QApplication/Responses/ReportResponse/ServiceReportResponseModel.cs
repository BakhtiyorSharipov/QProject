namespace QApplication.Responses.ReportResponse;

public class ServiceReportResponseModel
{
    public List<ServiceReportItemResponseModel> Services { get; set; } = new();
    
    public int TotalServices { get; set; }

}