using QApplication.Requests.CompanyRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface ICompanyService
{
    Task<IEnumerable<CompanyResponseModel>> GetAllAsync(int pageList, int pageNumber);

    Task<IEnumerable<CompanyResponseModel>> GetAllCompaniesAsync();
    Task<CompanyResponseModel> GetByIdAsync(int id);
    Task<CompanyResponseModel> AddAsync(CompanyRequestModel request);
    Task<CompanyResponseModel> UpdateAsync(int id, CompanyRequestModel requestModel);

    Task<bool> DeleteAsync(int id);
}