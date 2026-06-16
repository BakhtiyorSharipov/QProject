using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QAuthService.Contracts.Events.EmployeeEvent;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeResponseModel>
{
    private readonly ILogger<UpdateEmployeeCommand> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateEmployeeCommandHandler(ILogger<UpdateEmployeeCommand> logger, IUserServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<EmployeeResponseModel> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating employee with Id {employeeId}", request.Id);

        var dbEmployee = await _dbContext.Employees.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbEmployee == null)
        {
            _logger.LogWarning("Employee with Id {employeeId} not found for updating", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }
        
        dbEmployee.FirstName = request.Firstname;
        dbEmployee.LastName = request.Lastname;
        dbEmployee.Position = request.Position;
        dbEmployee.PhoneNumber = request.PhoneNumber;

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Employee with Id {dbEmployee.Id} updated successfully.", dbEmployee.Id);

        await _publishEndpoint.Publish(new EmployeeUpdatedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            CompanyId = dbEmployee.CompanyId,
            BranchId = dbEmployee.BranchId,
            ServiceId = dbEmployee.ServiceId,
            EmployeeId = dbEmployee.Id,
            FirstName = dbEmployee.FirstName,
            LastName = dbEmployee.LastName,
            Position = dbEmployee.Position,
            PhoneNumber = dbEmployee.PhoneNumber
        }, cancellationToken);
        
        var response = new EmployeeResponseModel()
        {
            Id = dbEmployee.Id,
            CompanyId = dbEmployee.CompanyId,
            BranchId = dbEmployee.BranchId,
            ServiceId = dbEmployee.ServiceId?? 0,
            FirstName = dbEmployee.FirstName,
            LastName = dbEmployee.LastName,
            Position = dbEmployee.Position,
            PhoneNumber = dbEmployee.PhoneNumber,
        };

        return response;
    }
}