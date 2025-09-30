using QApplication.Requests.EmployeeRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IEmployeeService
{
    IEnumerable<EmployeeResponseModel> GetAll(int pageList, int pageNumber);
    EmployeeResponseModel GetById(int id);
    EmployeeResponseModel Add(EmployeeRequestModel request);
    EmployeeResponseModel Update(int id, EmployeeRequestModel request);
    bool Delete(int id);
}