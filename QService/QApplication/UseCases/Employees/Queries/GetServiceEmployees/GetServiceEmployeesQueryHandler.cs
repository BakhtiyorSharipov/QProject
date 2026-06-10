using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QApplication.UseCases.Employees.Queries.GetBranchEmployees;
using QBranchService.Contracts.Interfaces;
using QBranchService.Contracts.Requests;

namespace QApplication.UseCases.Employees.Queries.GetServiceEmployees;

public class GetServiceEmployeesQueryHandler: IRequestHandler<GetServiceEmployeesQuery, PagedResponse<EmployeeInfoResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetBranchEmployeesQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IBranchService _branchService;

    public GetServiceEmployeesQueryHandler(ILogger<GetBranchEmployeesQueryHandler> logger, IQueueApplicationDbContext dbContext, IBranchService branchService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _branchService = branchService;
    }

    public async Task<PagedResponse<EmployeeInfoResponseModel>> Handle(GetServiceEmployeesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all branch employees. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            PageSize);


        var company = await _branchService.CheckCompanyId(new CompanyRequest
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            RequestedAt = DateTime.UtcNow
        });
        
        if (!company.IsValid)
        {
            _logger.LogError("Company with id {CompanyId} not found", request.CompanyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"Company with Id {request.CompanyId} not found");
        }
        
        var service = await _branchService.CheckCompanyServiceId(new CompanyServiceRequest
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            CompanyServiceId = request.ServiceId,
            RequestedAt = DateTime.UtcNow
        });

        if (!service.IsValid)
        {
            _logger.LogError("Service with id {ServiceId} not found", request.ServiceId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"Service with Id {request.ServiceId} not found");
        }

        var totalCount = await _dbContext.Employees
            .Where(s=>s.BranchId== request.ServiceId && s.CompanyId == request.CompanyId)
            .CountAsync(cancellationToken);

        var dbEmployees =await  _dbContext.Employees
            .Where(s=>s.BranchId== request.ServiceId && s.CompanyId == request.CompanyId)
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        var response = new List<EmployeeInfoResponseModel>();

        foreach (var employee in dbEmployees)
        {
            var reviews = await _dbContext.Reviews
                .Include(s => s.Queue)
                .Where(s => s.Queue.EmployeeId == employee.Id)
                .ToListAsync(cancellationToken);

            double averageRating = 0;

            if (reviews.Any())
            {
                averageRating = reviews.Average(s => s.Grade);
            }
            
            var complaints = await _dbContext.Complaints
                .Include(s => s.Queue)
                .Where(s => s.Queue.EmployeeId == employee.Id)
                .ToListAsync(cancellationToken);
            
            response.Add(new EmployeeInfoResponseModel
            {
                Id = employee.Id,
                CompanyId = employee.CompanyId,
                BranchId = employee.BranchId,
                ServiceId = employee.ServiceId ?? 0,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Position = employee.Position,
                PhoneNumber = employee.PhoneNumber,
                TotalReviews = reviews.Count,
                AverageRating = averageRating,
                TotalComplaints = complaints.Count
            });
            
        }
        
        _logger.LogInformation("Fetched {employeeCount} employees.", response.Count);

        return new PagedResponse<EmployeeInfoResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };
        
    }
}