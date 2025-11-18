using QApplication.Requests.CompanyRequest;
using QApplication.Requests.CustomerRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseModel>> GetAllAsync(int pageList, int pageNumber);
    Task<IEnumerable<CustomerResponseModel>> GetAllCustomerByCompanyAsync(int companyId);
    Task<CustomerResponseModel> GetByIdAsync(int id);
    Task<CustomerResponseModel> AddAsync(CustomerRequestModel request);
    Task<CustomerResponseModel> UpdateAsync(int id, CustomerRequestModel request);
    Task<bool> DeleteAsync(int id);
}