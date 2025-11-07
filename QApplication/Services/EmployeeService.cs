using System.Net;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<EmployeeService> _logger;
    public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IEnumerable<EmployeeResponseModel> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting all employees. PageNumber: {pageNumber}, PageList: {pageList}", pageNumber, pageList);
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
        
        _logger.LogInformation("Fetched {employeeCount} employees.", response.Count);
        return response;
    }

    public IEnumerable<EmployeeResponseModel> GetEmployeesByCompany(int companyId)
    {
        _logger.LogInformation("Getting employees by company Id {companyId}", companyId);
        var dbEmployees = _repository.GetEmployeeByCompany(companyId);
        if (!dbEmployees.Any())
        {   
            _logger.LogWarning("No employees found for this company Id {companyId}", companyId);
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

        _logger.LogInformation("{employeeCount} employees found for this company Id {companyId}", response.Count, companyId);
        return response;
    }

    public EmployeeResponseModel GetById(int id)
    {
        _logger.LogInformation("Getting employee with Id {employeeId}", id);
        var dbEmployee = _repository.FindById(id);
        if (dbEmployee == null)
        {
            _logger.LogWarning("No employee found for this Id {employeeId}", id);
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
        
        _logger.LogInformation("Employee with Id {employeeId} fetched successfully", id);
        return response;
    }

    public EmployeeResponseModel Add(EmployeeRequestModel request)
    {
        _logger.LogInformation("Adding new Employee with name {employeeName}", request.FirstName);
        var requestToCreate = request as CreateEmployeeRequest;
        if (requestToCreate == null)
        {
            _logger.LogError("Invalid request model while adding employee");
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
        
        _logger.LogInformation("Employee {employeeName} added successfully with Id {employeeId}", employee.FirstName, employee.Id);
        
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
        _logger.LogInformation("Updating employee with Id {employeeId}", id);
        var dbEmployee = _repository.FindById(id);
        if (dbEmployee == null)
        {
            _logger.LogWarning("Employee with Id {employeeId} not found for updating",id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        var requestToUpdate = request as UpdateEmployeeRequest;
        if (requestToUpdate == null)
        {
            _logger.LogError("Invalid request model while updating employee with Id {id}", id);
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
        _logger.LogInformation("Employee with Id {dbEmployee.Id} updated successfully.", dbEmployee.Id);
        
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
        _logger.LogInformation("Deleting employee with Id {id}", id);
        var dbEmployee = _repository.FindById(id);
        if (dbEmployee == null)
        {
            _logger.LogWarning("Employee with Id {id} not found for deleting", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        _repository.Delete(dbEmployee);
        _repository.SaveChanges();
        
        _logger.LogInformation("Employee with Id {id} deleted successfully", id);
        return true;
    }
}