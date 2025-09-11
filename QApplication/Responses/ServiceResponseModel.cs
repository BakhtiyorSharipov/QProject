namespace QApplication.Responses;

public class ServiceResponseModel: BaseResponse
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string ServiceName { get; set; }
    public string ServiceDescription { get; set; }
}