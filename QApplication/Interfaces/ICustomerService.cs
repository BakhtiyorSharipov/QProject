using QApplication.Requests.CustomerRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface ICustomerService: IBaseService<CustomerEntity, CustomerResponseModel, CustomerRequestModel>
{
    
}