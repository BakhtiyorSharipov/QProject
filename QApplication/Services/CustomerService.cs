using System.Globalization;
using System.Net;
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

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<CustomerResponseModel> GetAll(int pageList, int pageNumber)
    {
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

        return response;
    }

    public IEnumerable<CustomerResponseModel> GetAllCustomerByCompany(int companyId)
    {
        var dbCustomer = _repository.GetAllCustomersByCompany(companyId);
        if (!dbCustomer.Any())
        {
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

        return response;
    }

    public CustomerResponseModel GetById(int id)
    {
        var dbCustomer = _repository.FindById(id);
        if (dbCustomer==null)
        {
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

        return response;
    }
    
    public CustomerResponseModel Add(CustomerRequestModel request)
    {
        var requestToCreate = request as CreateCustomerRequest;
        if (requestToCreate==null)
        {
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
        var dbCustomer = _repository.FindById(id);
        if (dbCustomer==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var requestToUpdate = request as UpdateCustomerRequest;
        if (requestToUpdate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(CustomerEntity));
        }

        dbCustomer.FirstName = requestToUpdate.FirstName;
        dbCustomer.LastName = requestToUpdate.LastName;
        dbCustomer.EmailAddress = requestToUpdate.EmailAddress;
        dbCustomer.PhoneNumber = requestToUpdate.PhoneNumber;
        dbCustomer.Password = requestToUpdate.Password;
        
        _repository.Update(dbCustomer);
        _repository.SaveChanges();

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
        var dbCustomer = _repository.FindById(id);
        if (dbCustomer== null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }
        
        _repository.Delete(dbCustomer);
        _repository.SaveChanges();

        return true;
    }
}