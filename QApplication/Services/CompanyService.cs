using System.Net;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.CompanyRequest;
using QApplication.Responses;
using QDomain.Models;
using Microsoft.EntityFrameworkCore;

namespace QApplication.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(ICompanyRepository repository, ILogger<CompanyService> logger)
    {
        _repository = repository;
        _logger = logger;
    }


    public async Task<IEnumerable<CompanyResponseModel>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting all companies. PageNumber: {pageNumber}, PageList: {pageList}", pageNumber,
            pageList);

        var dbCompanies = await _repository.GetAll(pageList, pageNumber).ToListAsync();

        var response = dbCompanies.Select(company => new CompanyResponseModel()
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


    public async Task<IEnumerable<CompanyResponseModel>> GetAllCompaniesAsync()
    {
        _logger.LogInformation("Getting all companies without pagination");

        var dbCompany = await _repository.GetAllCompanies().ToListAsync();
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


    public async Task<CompanyResponseModel> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting company with Id {CompanyId}", id);
        var dbCompany = await _repository.FindByIdAsync(id);
        if (dbCompany == null)
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


    public async Task<CompanyResponseModel> AddAsync(CompanyRequestModel request)
    {
        _logger.LogInformation("Adding new company with Name {companyName}", request.CompanyName);
        var parsedToCreate = request as CreateCompanyRequest;
        if (parsedToCreate == null)
        {
            _logger.LogError("Invalid request model while adding a company");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CompanyEntity));
        }

        var company = new CompanyEntity()
        {
            CompanyName = parsedToCreate.CompanyName,
            Address = parsedToCreate.Address,
            EmailAddress = parsedToCreate.EmailAddress,
            PhoneNumber = parsedToCreate.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(company);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Company {companyName} added successfully with Id {companyId}", company.CompanyName,
            company.Id);
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


    public async Task<CompanyResponseModel> UpdateAsync(int id, CompanyRequestModel requestModel)
    {
        _logger.LogInformation("Updating company with Id {companyId}", id);
        var dbCompany = await _repository.FindByIdAsync(id);
        if (dbCompany == null)
        {
            _logger.LogWarning("Company with Id {companyId} not found for updating.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        var requestToUpdate = requestModel as UpdateCompanyRequest;
        if (requestToUpdate == null)
        {
            _logger.LogError("Invalid request model while updating company with Id {companyId}", id);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CompanyEntity));
        }


        dbCompany.CompanyName = requestToUpdate.CompanyName;
        dbCompany.Address = requestToUpdate.Address;
        dbCompany.EmailAddress = requestToUpdate.EmailAddress;
        dbCompany.PhoneNumber = requestToUpdate.PhoneNumber;


        _repository.Update(dbCompany);
        await _repository.SaveChangesAsync();

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


    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting company with Id {companyId}", id);

        var dbCompany = await _repository.FindByIdAsync(id);
        if (dbCompany == null)
        {
            _logger.LogWarning("Company with Id {companyId} not found for deleting", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        _repository.Delete(dbCompany);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Company with Id {companyId} deleted successfully", id);
        return true;
    }
}