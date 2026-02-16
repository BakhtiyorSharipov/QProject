namespace QApplication.Responses.ReportResponse;

public class CustomerReportResponseModel: BaseResponse
{
    public int TotalCustomers { get; set; }
    public int BlockedCustomers { get; set; }
    public int DidNotComeCustomers { get; set; }
}