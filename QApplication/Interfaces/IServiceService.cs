using QApplication.Requests.ServiceRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IServiceService
{

    Task<IEnumerable<ServiceResponseModel>> GetAllAsync(int pageList, int pageNumber);
    
    Task<IEnumerable<ServiceResponseModel>> GetAllServicesByCompanyAsync(int companyId);
    Task<ServiceResponseModel> GetByIdAsync(int id);
    Task<ServiceResponseModel> AddAsync(ServiceRequestModel request);
    Task<ServiceResponseModel> UpdateAsync(int id, ServiceRequestModel request);
    Task<bool> DeleteAsync(int id);
}