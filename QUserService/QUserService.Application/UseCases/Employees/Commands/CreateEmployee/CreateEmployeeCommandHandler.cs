using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using QAuthService.Contracts.Events.EmployeeEvent;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler: IRequestHandler<CreateEmployeeCommand, EmployeeResponseModel>
{
    private readonly ILogger<CreateEmployeeCommandHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateEmployeeCommandHandler(ILogger<CreateEmployeeCommandHandler> logger, IUserServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
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

        await _publishEndpoint.Publish(new EmployeeCreatedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            CompanyId = employee.CompanyId,
            BranchId = employee.BranchId,
            EmployeeId = employee.Id,
            ServiceId = employee.ServiceId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            PhoneNumber = employee.PhoneNumber
        }, cancellationToken);

        var response = new EmployeeResponseModel()
        {
            Id = employee.Id,
            CompanyId = employee.CompanyId,
            BranchId = employee.BranchId,
            ServiceId = employee.ServiceId.Value,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            PhoneNumber = employee.PhoneNumber,
        };

        return response;
    }
}