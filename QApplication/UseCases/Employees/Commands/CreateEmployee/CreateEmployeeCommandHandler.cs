using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler: IRequestHandler<CreateEmployeeCommand, EmployeeResponseModel>
{
    private readonly ILogger<CreateEmployeeCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public CreateEmployeeCommandHandler(ILogger<CreateEmployeeCommandHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<EmployeeResponseModel> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding new Employee with name {employeeName}", request.Firstname);
        

        var employee = new EmployeeEntity()
        {
            FirstName = request.Firstname,
            LastName = request.Lastname,
            Position = request.Position,
            PhoneNumber = request.PhoneNumber,
            ServiceId = request.ServiceId,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Employee {employeeName} added successfully with Id {employeeId}", employee.FirstName,
            employee.Id);

        var response = new EmployeeResponseModel()
        {
            Id = employee.Id,
            ServiceId = employee.ServiceId.Value,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            PhoneNumber = employee.PhoneNumber,
        };

        return response;
    }
}