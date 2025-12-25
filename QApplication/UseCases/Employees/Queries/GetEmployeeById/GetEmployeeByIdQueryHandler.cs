using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler: IRequestHandler<GetEmployeeByIdQuery, EmployeeResponseModel>
{
    private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetEmployeeByIdQueryHandler(ILogger<GetEmployeeByIdQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<EmployeeResponseModel> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting employee with Id {EmployeeUd}", request.Id);

        var dbEmployee = await _dbContext.Employees.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbEmployee==null)
        {
            _logger.LogWarning("Employee with Id {EmployeeId} not found", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        var response = new EmployeeResponseModel
        {
            Id = dbEmployee.Id,
            ServiceId = dbEmployee.ServiceId.HasValue? dbEmployee.ServiceId.Value :0,
            FirstName = dbEmployee.FirstName,
            LastName = dbEmployee.LastName,
            Position = dbEmployee.Position,
            PhoneNumber = dbEmployee.PhoneNumber
        };

        _logger.LogInformation("Employee with Id {EmployeeId} fetched successfully", request.Id);
        return response;
    }
}