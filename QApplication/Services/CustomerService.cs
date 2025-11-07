using System.Globalization;
using System.Net;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.CompanyRequest;
using QApplication.Requests.CustomerRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class CustomerService: ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;
    public CustomerService(ICustomerRepository repository, ILogger<CustomerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IEnumerable<CustomerResponseModel> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting all customers, PageNumber:{pageNumber}, PageList: {pageList}", pageNumber, pageList);
        var dbCustomer = _repository.GetAll(pageList, pageNumber);
        var response = dbCustomer.Select(customer => new CustomerResponseModel()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            EmailAddress = customer.EmailAddress,
            PhoneNumber = customer.PhoneNumber,
            Password = customer.Password
        }).ToList();
        
        _logger.LogInformation("Fetched {response.Count} customers.", response.Count);
        return response;
    }

    public IEnumerable<CustomerResponseModel> GetAllCustomerByCompany(int companyId)
    {
        _logger.LogInformation("Getting customer by company Id {companyId}.", companyId);
        var dbCustomer = _repository.GetAllCustomersByCompany(companyId);
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
            EmailAddress = customer.EmailAddress,
            PhoneNumber = customer.PhoneNumber,
            Password = customer.Password
        }).ToList();

        _logger.LogInformation("{response.Count} customers found for this company Id {companyId}", response.Count, companyId);
        return response;
    }

    public CustomerResponseModel GetById(int id)
    {
        _logger.LogInformation("Getting customer with Id {id}.", id);
        var dbCustomer = _repository.FindById(id);
        if (dbCustomer==null)
        {
            _logger.LogWarning("No customer found for this Id {id}.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var response = new CustomerResponseModel()
        {
            Id = dbCustomer.Id,
            FirstName = dbCustomer.FirstName,
            LastName = dbCustomer.LastName,
            EmailAddress = dbCustomer.EmailAddress,
            PhoneNumber = dbCustomer.PhoneNumber,
            Password = dbCustomer.Password
        };
        
        _logger.LogInformation("Customer with Id {id} fetched successfully.", id);
        return response;
    }
    
    public CustomerResponseModel Add(CustomerRequestModel request)
    {
        _logger.LogInformation("Adding new customer with name {request.FirstName}", request.FirstName);
        var requestToCreate = request as CreateCustomerRequest;
        if (requestToCreate==null)
        {
            _logger.LogError("Invalid request model while adding customer.");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CustomerEntity));
        }

        var customer = new CustomerEntity()
        {
            FirstName = requestToCreate.FirstName,
            LastName = requestToCreate.LastName,
            EmailAddress = requestToCreate.EmailAddress,
            PhoneNumber = requestToCreate.PhoneNumber,
            Password = requestToCreate.Password
        };
        
        _repository.Add(customer);
        _repository.SaveChanges();

        _logger.LogInformation("Customer {customer.FirstName} added successfully with Id {customer.Id}", customer.FirstName, customer.Id);
        var response = new CustomerResponseModel()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            EmailAddress = customer.EmailAddress,
            PhoneNumber = customer.PhoneNumber,
            Password = customer.PhoneNumber
        };

        return response;
    }

    public CustomerResponseModel Update(int id, CustomerRequestModel request)
    {
        _logger.LogInformation("Updating customer with Id {id}.", id);
        var dbCustomer = _repository.FindById(id);
        if (dbCustomer==null)
        {
            _logger.LogWarning("Customer with Id {id} not found for updating.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var requestToUpdate = request as UpdateCustomerRequest;
        if (requestToUpdate==null)
        {
            _logger.LogError("Invalid request model while updating customer with Id {id}.", id);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CustomerEntity));
        }

        dbCustomer.FirstName = requestToUpdate.FirstName;
        dbCustomer.LastName = requestToUpdate.LastName;
        dbCustomer.EmailAddress = requestToUpdate.EmailAddress;
        dbCustomer.PhoneNumber = requestToUpdate.PhoneNumber;
        dbCustomer.Password = requestToUpdate.Password;
        
        _repository.Update(dbCustomer);
        _repository.SaveChanges();
        
        _logger.LogInformation("Customer with Id {id} updated successfully.", id);
        
        var response = new CustomerResponseModel()
        {
            Id = dbCustomer.Id,
            FirstName = dbCustomer.FirstName,
            LastName = dbCustomer.LastName,
            EmailAddress = dbCustomer.EmailAddress,
            PhoneNumber = dbCustomer.PhoneNumber,
            Password = dbCustomer.Password
        };

        return response;
    }

    public bool Delete(int id)
    {
        _logger.LogInformation("Deleting customer with Id {id}", id);
        var dbCustomer = _repository.FindById(id);
        if (dbCustomer== null)
        {
            _logger.LogWarning("Customer with Id {id} not for deleting.", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }
        
        _repository.Delete(dbCustomer);
        _repository.SaveChanges();
        
        _logger.LogInformation("Customer with Id {id} deleted successfully.", id);
        
        return true;
    }
}