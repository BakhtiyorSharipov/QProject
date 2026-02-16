using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Employees.Queries.GetAllEmployees;

public class GetAllEmployeesQueryHandler: IRequestHandler<GetAllEmployeesQuery, PagedResponse<EmployeeResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllEmployeesQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetAllEmployeesQueryHandler(ILogger<GetAllEmployeesQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<EmployeeResponseModel>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all employees. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            PageSize);

        var totalCount = await _dbContext.Employees.CountAsync(cancellationToken);

        var dbEmployees =await  _dbContext.Employees
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        var response = dbEmployees.Select(employee => new EmployeeResponseModel
        {
            Id = employee.Id,
            ServiceId = employee.ServiceId ?? 0,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            PhoneNumber = employee.PhoneNumber
        }).ToList();
        
        _logger.LogInformation("Fetched {employeeCount} employees.", response.Count);

        return new PagedResponse<EmployeeResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}