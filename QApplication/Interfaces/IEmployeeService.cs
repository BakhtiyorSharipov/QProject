using QApplication.Requests.EmployeeRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IEmployeeService
{
    IEnumerable<EmployeeResponseModel> GetAll(int pageList, int pageNumber);
    IEnumerable<EmployeeResponseModel> GetEmployeesByCompany(int companyId);
    EmployeeResponseModel GetById(int id);
    EmployeeResponseModel Add(EmployeeRequestModel request);
    EmployeeResponseModel Update(int id, EmployeeRequestModel request);
    bool Delete(int id);
}