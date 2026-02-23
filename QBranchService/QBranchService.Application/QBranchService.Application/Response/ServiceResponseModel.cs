namespace QBranchService.Application.Response;

public class ServiceResponseModel
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string ServiceName { get; set; }
    public string ServiceDescription { get; set; }
}