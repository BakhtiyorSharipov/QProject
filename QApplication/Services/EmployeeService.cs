using System.Net;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.CompanyRequest;
using QApplication.Requests.EmployeeRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<EmployeeResponseModel> GetAll(int pageList, int pageNumber)
    {
        var dbEmployee = _repository.GetAll(pageList, pageNumber);
        var response = dbEmployee.Select(employee => new EmployeeResponseModel()
        {
            Id = employee.Id,
            ServiceId = employee.ServiceId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            EmailAddress = employee.EmailAddress,
            PhoneNumber = employee.PhoneNumber,
            Password = employee.Password
        }).ToList();

        return response;
    }

    public IEnumerable<EmployeeResponseModel> GetEmployeesByCompany(int companyId)
    {
        var dbEmployees = _repository.GetEmployeeByCompany(companyId);
        if (!dbEmployees.Any())
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        var response = dbEmployees.Select(dbEmployee => new EmployeeResponseModel
        {
            Id = dbEmployee.Id,
            ServiceId = dbEmployee.ServiceId,
            FirstName = dbEmployee.FirstName,
            LastName = dbEmployee.LastName,
            Position = dbEmployee.Position,
            EmailAddress = dbEmployee.EmailAddress,
            PhoneNumber = dbEmployee.PhoneNumber,
            Password = dbEmployee.Password
        }).ToList();


        return response;
    }

    public EmployeeResponseModel GetById(int id)
    {
        var dbEmployee = _repository.FindById(id);
        if (dbEmployee == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        var response = new EmployeeResponseModel()
        {
            Id = dbEmployee.Id,
            ServiceId = dbEmployee.ServiceId,
            FirstName = dbEmployee.FirstName,
            LastName = dbEmployee.LastName,
            Position = dbEmployee.Position,
            EmailAddress = dbEmployee.EmailAddress,
            PhoneNumber = dbEmployee.PhoneNumber,
            Password = dbEmployee.Password
        };

        return response;
    }

    public EmployeeResponseModel Add(EmployeeRequestModel request)
    {
        var requestToCreate = request as CreateEmployeeRequest;
        if (requestToCreate == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(EmployeeEntity));
        }

        var employee = new EmployeeEntity()
        {
            FirstName = requestToCreate.FirstName,
            LastName = requestToCreate.LastName,
            Position = requestToCreate.Position,
            EmailAddress = request.EmailAddress,
            PhoneNumber = requestToCreate.PhoneNumber,
            Password = requestToCreate.Password,
            ServiceId = requestToCreate.ServiceId
        };

        _repository.Add(employee);
        _repository.SaveChanges();

        var response = new EmployeeResponseModel()
        {
            Id = employee.Id,
            ServiceId = employee.ServiceId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            EmailAddress = employee.EmailAddress,
            PhoneNumber = employee.PhoneNumber,
            Password = employee.Password
        };

        return response;
    }

    public EmployeeResponseModel Update(int id, EmployeeRequestModel request)
    {
        var dbEmployee = _repository.FindById(id);
        if (dbEmployee == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        var requestToUpdate = request as UpdateEmployeeRequest;
        if (requestToUpdate == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(EmployeeEntity));
        }

        dbEmployee.FirstName = requestToUpdate.FirstName;
        dbEmployee.LastName = requestToUpdate.LastName;
        dbEmployee.Position = requestToUpdate.Position;
        dbEmployee.EmailAddress = requestToUpdate.EmailAddress;
        dbEmployee.PhoneNumber = requestToUpdate.PhoneNumber;
        dbEmployee.Password = requestToUpdate.Password;

        _repository.Update(dbEmployee);
        _repository.SaveChanges();

        var response = new EmployeeResponseModel()
        {
            Id = dbEmployee.Id,
            ServiceId = dbEmployee.ServiceId,
            FirstName = dbEmployee.FirstName,
            LastName = dbEmployee.LastName,
            Position = dbEmployee.Position,
            EmailAddress = dbEmployee.EmailAddress,
            PhoneNumber = dbEmployee.PhoneNumber,
            Password = dbEmployee.Password
        };

        return response;
    }

    public bool Delete(int id)
    {
        var dbEmployee = _repository.FindById(id);
        if (dbEmployee == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        _repository.Delete(dbEmployee);
        _repository.SaveChanges();
        return true;
    }
}