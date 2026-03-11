namespace QBranchService.Application.Requests;

public class UpdateCompanyServiceRequest
{
    public int CompanyId { get; set; }
    public string ServiceName { get; set; }
    public string ServiceDescription { get; set; }
}