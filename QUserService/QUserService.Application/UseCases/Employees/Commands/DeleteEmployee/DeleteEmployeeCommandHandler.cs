using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QAuthService.Contracts.Events.EmployeeEvent;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler: IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly ILogger<DeleteEmployeeCommandHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteEmployeeCommandHandler(ILogger<DeleteEmployeeCommandHandler> logger, IUserServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting employee with Id {id}", request.Id);

        var dbEmployee = await _dbContext.Employees.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbEmployee== null)
        {
            _logger.LogWarning("Employee with Id {id} not found for deleting", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }
        
        _dbContext.Employees.Remove(dbEmployee);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Employee with Id {id} deleted successfully", request.Id);

        await _publishEndpoint.Publish(new EmployeeDeletedEvent
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
        
        return true;
    }
}