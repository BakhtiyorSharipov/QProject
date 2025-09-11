using QApplication.Requests.CompanyRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface ICompanyService
{
    IEnumerable<CompanyResponseModel> GetAll(int pageList, int pageNumber);
    CompanyResponseModel GetById(int id);
    CompanyResponseModel Add(CompanyRequestModel request);
    CompanyResponseModel Update(int id, CompanyRequestModel request);
    bool Delete(int id);
}