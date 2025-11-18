using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.ServiceRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _repository;
    private readonly ILogger<ServiceService> _logger;

    public ServiceService(IServiceRepository repository, ILogger<ServiceService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<ServiceResponseModel>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting all services. PageNumber {pageNumber}, PageList: {pageList}", pageNumber,
            pageList);
        var dbService = await _repository.GetAll(pageList, pageNumber).ToListAsync();
        var response = dbService.Select(service => new ServiceResponseModel()
        {
            Id = service.Id,
            CompanyId = service.CompanyId,
            ServiceName = service.ServiceName,
            ServiceDescription = service.ServiceDescription
        }).ToList();

        _logger.LogInformation("Fetched {response.Count} services.", response.Count);
        return response;
    }

    public async Task<IEnumerable<ServiceResponseModel>> GetAllServicesByCompanyAsync(int companyId)
    {
        _logger.LogInformation("Getting services by company Id {companyId}", companyId);
        var dbServices = await _repository.GetAllServicesByCompany(companyId).ToListAsync();
        if (!dbServices.Any())
        {
            _logger.LogWarning("No service found for this company Id {companyId}", companyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = dbServices.Select(service => new ServiceResponseModel
        {
            Id = service.Id,
            CompanyId = service.CompanyId,
            ServiceName = service.ServiceName,
            ServiceDescription = service.ServiceDescription
        }).ToList();

        _logger.LogInformation("Fetched {response.Count} services for this company Id {companyId}", response.Count,
            companyId);
        return response;
    }

    public async Task<ServiceResponseModel> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting service by Id {id}", id);
        var dbService = await _repository.FindByIdAsync(id);
        if (dbService == null)
        {
            _logger.LogWarning("Service with Id {id} not found.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = new ServiceResponseModel()
        {
            Id = dbService.Id,
            CompanyId = dbService.CompanyId,
            ServiceName = dbService.ServiceName,
            ServiceDescription = dbService.ServiceDescription
        };

        _logger.LogInformation("Service with Id {id} fetched successfully.", id);
        return response;
    }

    public async Task<ServiceResponseModel> AddAsync(ServiceRequestModel request)
    {
        _logger.LogInformation("Adding new service with Name {request.ServiceName}", request.ServiceName);
        var requestToCreate = request as CreateServiceRequest;
        if (requestToCreate == null)
        {
            _logger.LogError("Invalid request model while adding new service.");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(ServiceEntity));
        }

        var service = new ServiceEntity()
        {
            CompanyId = requestToCreate.CompanyId,
            ServiceName = requestToCreate.ServiceName,
            ServiceDescription = requestToCreate.ServiceDescription
        };

        await _repository.AddAsync(service);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Service {service.ServiceName} added successfully with Id {service.Id}.",
            service.ServiceName);

        var response = new ServiceResponseModel()
        {
            Id = service.Id,
            CompanyId = service.CompanyId,
            ServiceName = service.ServiceName,
            ServiceDescription = service.ServiceDescription
        };

        return response;
    }

    public async Task<ServiceResponseModel> UpdateAsync(int id, ServiceRequestModel request)
    {
        _logger.LogInformation("Updating service with Id {id}.", id);
        var dbService = await _repository.FindByIdAsync(id);
        if (dbService == null)
        {
            _logger.LogWarning("Service with Id {id} not found for updating.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var requestToUpdate = request as UpdateServiceRequest;
        if (requestToUpdate == null)
        {
            _logger.LogError("Invalid request model while updating service with Id {id}", id);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(ServiceEntity));
        }

        dbService.CompanyId = requestToUpdate.CompanyId;
        dbService.ServiceName = requestToUpdate.ServiceName;
        dbService.ServiceDescription = requestToUpdate.ServiceDescription;

        _repository.Update(dbService);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Service with Id {id} updated successfully.", id);

        var response = new ServiceResponseModel()
        {
            Id = dbService.Id,
            CompanyId = dbService.CompanyId,
            ServiceName = dbService.ServiceName,
            ServiceDescription = dbService.ServiceDescription
        };

        return response;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting service with Id {id}", id);
        var dbService = await _repository.FindByIdAsync(id);
        if (dbService == null)
        {
            _logger.LogWarning("Service with Id {id} not found for deleting.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        _repository.Delete(dbService);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Service with Id {id} deleted successfully.", id);

        return true;
    }
}