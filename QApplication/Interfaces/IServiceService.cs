using QApplication.Requests.ServiceRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IServiceService
{
    IEnumerable<ServiceResponseModel> GetAll(int pageList, int pageNumber);
    IEnumerable<ServiceResponseModel> GetAllServicesByCompany(int companyId);
    ServiceResponseModel GetById(int id);
    ServiceResponseModel Add(ServiceRequestModel request);
    ServiceResponseModel Update(int id, ServiceRequestModel request);
    bool Delete(int id);
}