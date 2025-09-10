using QApplication.Requests.ServiceRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IServiceService: IBaseService<ServiceEntity, ServiceResponseModel, ServiceRequestModel>
{
    
}