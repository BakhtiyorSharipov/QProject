using QApplication.Requests.EmployeeRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IEmployeeService: IBaseService<EmployeeEntity, EmployeeResponseModel, EmployeeRequestModel>
{
    
}