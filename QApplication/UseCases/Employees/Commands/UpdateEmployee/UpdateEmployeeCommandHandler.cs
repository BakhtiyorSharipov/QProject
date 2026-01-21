using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Employees.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeResponseModel>
{
    private readonly ILogger<UpdateEmployeeCommand> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public UpdateEmployeeCommandHandler(ILogger<UpdateEmployeeCommand> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
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

        var response = new EmployeeResponseModel()
        {
            Id = dbEmployee.Id,
            ServiceId = dbEmployee.ServiceId.Value,
            FirstName = dbEmployee.FirstName,
            LastName = dbEmployee.LastName,
            Position = dbEmployee.Position,
            PhoneNumber = dbEmployee.PhoneNumber,
        };

        return response;
    }
}