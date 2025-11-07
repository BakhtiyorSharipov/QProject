using System.Net;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.CompanyRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class CompanyService: ICompanyService
{
    private readonly ICompanyRepository _repository;
    private readonly ILogger<CompanyService> _logger;
    public CompanyService(ICompanyRepository repository, ILogger<CompanyService> logger) 
    {
        _repository = repository;
        _logger = logger;
    }

    public IEnumerable<CompanyResponseModel> GetAll(int pageList, int pageNumber)
    {
        
        _logger.LogInformation("Getting all companies. PageNumber: {pageNumber}, PageList: {pageNumber}", pageNumber, pageList);
        var dbCompany = _repository.GetAll(pageList, pageNumber);
        var response = dbCompany.Select(company => new CompanyResponseModel()
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Address = company.Address,
            EmailAddress = company.EmailAddress,
            PhoneNumber = company.PhoneNumber
        }).ToList();

        _logger.LogInformation("Fetched {companyCount} companies.", response.Count);
        return response;
    }

    public IEnumerable<CompanyResponseModel> GetAllCompanies()
    {
        _logger.LogInformation("Getting all companies without pagination");

        var dbCompany = _repository.GetAllCompanies();
        if (!dbCompany.Any())
        {
            _logger.LogWarning("No companies found in the system");
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }
        
        var response = dbCompany.Select(company => new CompanyResponseModel()
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Address = company.Address,
            EmailAddress = company.EmailAddress,
            PhoneNumber = company.PhoneNumber
        }).ToList();
        _logger.LogInformation("Fetched {companyCount} companies.", response.Count);
        return response;
    }

    public CompanyResponseModel GetById(int id)
    {
        _logger.LogInformation("Getting company with Id {CompanyId}", id);
        var dbCompany = _repository.FindById(id);
        if (dbCompany==null)
        {
            _logger.LogWarning("Company with Id {Company} not found", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        var response = new CompanyResponseModel()
        {
            Id = dbCompany.Id,
            CompanyName = dbCompany.CompanyName,
            Address = dbCompany.Address,
            EmailAddress = dbCompany.EmailAddress,
            PhoneNumber = dbCompany.PhoneNumber
        };
        
      _logger.LogInformation("Company with Id {CompanyId} fetched successfully", id);

        return response;
    }

    public CompanyResponseModel Add(CompanyRequestModel request)
    {
        _logger.LogInformation("Adding new company with Name {companyName}", request.CompanyName);
        var parsedToCreate = request as CreateCompanyRequest;
        if (parsedToCreate==null)
        {   _logger.LogError("Invalid request model while adding a company");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CompanyEntity));
        }

        var company = new CompanyEntity()
        {
            CompanyName = parsedToCreate.CompanyName,
            Address = parsedToCreate.Address,
            EmailAddress = parsedToCreate.EmailAddress,
            PhoneNumber = parsedToCreate.PhoneNumber
        };
        
        _repository.Add(company);
        _repository.SaveChanges();
        _logger.LogInformation("Company {companyName} added successfully with Id {companyId}", company.CompanyName, company.Id);
        var response = new CompanyResponseModel()
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Address = company.Address,
            EmailAddress = company.EmailAddress,
            PhoneNumber = company.PhoneNumber
        };
        
        return response;
    }

    public CompanyResponseModel Update(int id, CompanyRequestModel request)
    {
        
        _logger.LogInformation("Updating company with Id {companyId}", id);
        var dbCompany = _repository.FindById(id);
        if (dbCompany==null)
        {   
            _logger.LogWarning("Company with Id {companyId} not found for updating.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        var requestToUpdate = request as UpdateCompanyRequest;
        if (requestToUpdate==null)
        {
            _logger.LogError("Invalid request model while updating company with Id {companyId}", id);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CompanyEntity));
        }


        dbCompany.CompanyName = requestToUpdate.CompanyName;
        dbCompany.Address = requestToUpdate.Address;
        dbCompany.EmailAddress = requestToUpdate.EmailAddress;
        dbCompany.PhoneNumber = requestToUpdate.PhoneNumber;
        
        
        _repository.Update(dbCompany);
        _repository.SaveChanges();

        _logger.LogInformation("Company with Id {companyId} updated successfully", id);
        
        var response = new CompanyResponseModel()
        {
            Id = dbCompany.Id,
            CompanyName = dbCompany.CompanyName,
            Address = dbCompany.Address,
            EmailAddress = dbCompany.EmailAddress,
            PhoneNumber = dbCompany.PhoneNumber
        };

        return response;

    }


    public bool Delete(int id)
    {
        
        _logger.LogInformation("Deleting company with Id {companyId}", id);
        
        var dbCompany = _repository.FindById(id);
        if (dbCompany==null)
        {
            _logger.LogWarning("Company with Id {companyId} not found for deleting", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }
        
        _repository.Delete(dbCompany);
        _repository.SaveChanges();
        _logger.LogInformation("Company with Id {companyId} deleted successfully", id);
        return true;
    }
}