using System.Globalization;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.CompanyRequest;
using QApplication.Requests.CustomerRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ICustomerRepository repository, ILogger<CustomerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerResponseModel>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting all customers, PageNumber:{pageNumber}, PageList: {pageList}", pageNumber,
            pageList);
        var dbCustomer = await _repository.GetAll(pageList, pageNumber).ToListAsync();
        var response = dbCustomer.Select(customer => new CustomerResponseModel()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
        }).ToList();

        _logger.LogInformation("Fetched {response.Count} customers.", response.Count);
        return response;
    }


    public async Task<IEnumerable<CustomerResponseModel>> GetAllCustomerByCompanyAsync(int companyId)
    {
        _logger.LogInformation("Getting customer by company Id {companyId}.", companyId);
        var dbCustomer = await _repository.GetAllCustomersByCompany(companyId).ToListAsync();
        if (!dbCustomer.Any())
        {
            _logger.LogWarning("No customers found for this company Id {companyId}", companyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var response = dbCustomer.Select(customer => new CustomerResponseModel
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
        }).ToList();

        _logger.LogInformation("{response.Count} customers found for this company Id {companyId}", response.Count,
            companyId);
        return response;
    }

    public async Task<CustomerResponseModel> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting customer with Id {id}.", id);
        var dbCustomer = await _repository.FindByIdAsync(id);
        if (dbCustomer == null)
        {
            _logger.LogWarning("No customer found for this Id {id}.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var response = new CustomerResponseModel()
        {
            Id = dbCustomer.Id,
            FirstName = dbCustomer.FirstName,
            LastName = dbCustomer.LastName,
            PhoneNumber = dbCustomer.PhoneNumber,
        };

        _logger.LogInformation("Customer with Id {id} fetched successfully.", id);
        return response;
    }

    public async Task<CustomerResponseModel> AddAsync(CustomerRequestModel request)
    {
        _logger.LogInformation("Adding new customer with name {request.FirstName}", request.FirstName);
        var requestToCreate = request as CreateCustomerRequest;
        if (requestToCreate == null)
        {
            _logger.LogError("Invalid request model while adding customer.");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CustomerEntity));
        }

        var customer = new CustomerEntity()
        {
            FirstName = requestToCreate.FirstName,
            LastName = requestToCreate.LastName,
            PhoneNumber = requestToCreate.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(customer);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Customer {customer.FirstName} added successfully with Id {customer.Id}",
            customer.FirstName, customer.Id);
        var response = new CustomerResponseModel()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
        };

        return response;
    }


    public async Task<CustomerResponseModel> UpdateAsync(int id, CustomerRequestModel request)
    {
        _logger.LogInformation("Updating customer with Id {id}.", id);
        var dbCustomer = await _repository.FindByIdAsync(id);
        if (dbCustomer == null)
        {
            _logger.LogWarning("Customer with Id {id} not found for updating.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var requestToUpdate = request as UpdateCustomerRequest;
        if (requestToUpdate == null)
        {
            _logger.LogError("Invalid request model while updating customer with Id {id}.", id);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CustomerEntity));
        }

        dbCustomer.FirstName = requestToUpdate.FirstName;
        dbCustomer.LastName = requestToUpdate.LastName;
        dbCustomer.PhoneNumber = requestToUpdate.PhoneNumber;

        _repository.Update(dbCustomer);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Customer with Id {id} updated successfully.", id);

        var response = new CustomerResponseModel()
        {
            Id = dbCustomer.Id,
            FirstName = dbCustomer.FirstName,
            LastName = dbCustomer.LastName,
            PhoneNumber = dbCustomer.PhoneNumber,
        };

        return response;
    }


    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting customer with Id {id}", id);
        var dbCustomer = await _repository.FindByIdAsync(id);
        if (dbCustomer == null)
        {
            _logger.LogWarning("Customer with Id {id} not for deleting.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        _repository.Delete(dbCustomer);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Customer with Id {id} deleted successfully.", id);

        return true;
    }
}