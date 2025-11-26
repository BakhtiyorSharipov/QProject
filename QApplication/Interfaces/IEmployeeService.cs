using QApplication.Requests.EmployeeRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeResponseModel>> GetAllAsync(int pageList, int pageNumber);
    Task<IEnumerable<EmployeeResponseModel>> GetEmployeesByCompanyAsync(int companyId);
    Task<EmployeeResponseModel> GetByIdAsync(int id);
    Task<EmployeeResponseModel> AddAsync(EmployeeRequestModel request);
    Task<EmployeeResponseModel> UpdateAsync(int id, EmployeeRequestModel request);
    Task<bool> DeleteAsync(int id);
    
}