using QApplication.Requests.CompanyRequest;
using QApplication.Requests.CustomerRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface ICustomerService
{
    IEnumerable<CustomerResponseModel> GetAll(int pageList, int pageNumber);
    IEnumerable<CustomerResponseModel> GetAllCustomerByCompany(int companyId);
    CustomerResponseModel GetById(int id);
    CustomerResponseModel Add(CustomerRequestModel request);
    CustomerResponseModel Update(int id, CustomerRequestModel request);
    bool Delete(int id);
}