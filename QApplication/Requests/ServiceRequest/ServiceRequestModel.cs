namespace QApplication.Requests.ServiceRequest;

public class ServiceRequestModel: BaseRequest
{
    public int CompanyId { get; set; }
    public string ServiceName { get; set; }
    public string ServiceDescription { get; set; }
}