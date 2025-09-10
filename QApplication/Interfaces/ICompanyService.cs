using QApplication.Requests.CompanyRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface ICompanyService: IBaseService<CompanyEntity, CompanyResponseModel, CompanyRequestModel>
{
    
}