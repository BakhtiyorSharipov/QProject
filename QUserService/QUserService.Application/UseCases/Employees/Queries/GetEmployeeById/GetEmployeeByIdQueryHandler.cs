using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler: IRequestHandler<GetEmployeeByIdQuery, EmployeeResponseModel>
{
    private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;

    public GetEmployeeByIdQueryHandler(ILogger<GetEmployeeByIdQueryHandler> logger, IUserServiceApplicationDbContext dbContext)
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
            CompanyId = dbEmployee.CompanyId,
            BranchId = dbEmployee.BranchId,
            ServiceId = dbEmployee.ServiceId?? 0,
            FirstName = dbEmployee.FirstName,
            LastName = dbEmployee.LastName,
            Position = dbEmployee.Position,
            PhoneNumber = dbEmployee.PhoneNumber
        };

        _logger.LogInformation("Employee with Id {EmployeeId} fetched successfully", request.Id);
        return response;
    }
}