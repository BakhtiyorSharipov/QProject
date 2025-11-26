using System.Net;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.ServiceRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class ServiceService: IServiceService
{
    private readonly IServiceRepository _repository;

    public ServiceService(IServiceRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<ServiceResponseModel> GetAll(int pageList, int pageNumber)
    {
        var dbService = _repository.GetAll(pageList, pageNumber);
        if (dbService==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = dbService.Select(service => new ServiceResponseModel()
        {
            Id=service.Id,
            CompanyId = service.CompanyId,
            ServiceName = service.ServiceName,
            ServiceDescription = service.ServiceDescription
        }).ToList();

        return response;
    }

    public IEnumerable<ServiceResponseModel> GetAllServicesByCompany(int companyId)
    {
        var dbServices = _repository.GetAllServicesByCompany(companyId);
        if (!dbServices.Any())
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));

        }

        var response = dbServices.Select(service => new ServiceResponseModel
        {
            Id = service.Id,
            CompanyId = service.CompanyId,
            ServiceName = service.ServiceName,
            ServiceDescription = service.ServiceDescription
        }).ToList();

        return response;
    }

    public ServiceResponseModel GetById(int id)
    {
        var dbService = _repository.FindById(id);
        if (dbService==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = new ServiceResponseModel()
        {
            Id=dbService.Id,
            CompanyId = dbService.CompanyId,
            ServiceName = dbService.ServiceName,
            ServiceDescription = dbService.ServiceDescription
        };

        return response;
    }

    public ServiceResponseModel Add(ServiceRequestModel request)
    {
        var requestToCreate = request as CreateServiceRequest;
        if (requestToCreate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(ServiceEntity));
        }

        var service = new ServiceEntity()
        {
            CompanyId = requestToCreate.CompanyId,
            ServiceName = requestToCreate.ServiceName,
            ServiceDescription = requestToCreate.ServiceDescription
        };
        
        _repository.Add(service);
        _repository.SaveChanges();

        var response = new ServiceResponseModel()
        {
            Id = service.Id,
            CompanyId = service.CompanyId,
            ServiceName = service.ServiceName,
            ServiceDescription = service.ServiceDescription
        };

        return response;
    }

    public ServiceResponseModel Update(int id, ServiceRequestModel request)
    {
        var dbService = _repository.FindById(id);
        if (dbService== null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var requestToUpdate = request as UpdateServiceRequest;
        if (requestToUpdate== null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(ServiceEntity));
        }

        dbService.CompanyId = requestToUpdate.CompanyId;
        dbService.ServiceName = requestToUpdate.ServiceName;
        dbService.ServiceDescription = requestToUpdate.ServiceDescription;
        
        _repository.Update(dbService);
        _repository.SaveChanges();

        var response = new ServiceResponseModel()
        {
            Id=dbService.Id,
            CompanyId = dbService.CompanyId,
            ServiceName = dbService.ServiceName,
            ServiceDescription = dbService.ServiceDescription
        };

        return response;
    }

    public bool Delete(int id)
    {
        var dbService = _repository.FindById(id);
        if (dbService== null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }
        
        _repository.Delete(dbService);
        _repository.SaveChanges();

        return true;
    }
}