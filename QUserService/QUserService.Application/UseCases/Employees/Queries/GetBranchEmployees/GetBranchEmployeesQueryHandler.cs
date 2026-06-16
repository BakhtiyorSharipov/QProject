using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Contracts.Interfaces;
using QBranchService.Contracts.Requests;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Employees.Queries.GetBranchEmployees;

public class
    GetBranchEmployeesQueryHandler : IRequestHandler<GetBranchEmployeesQuery, PagedResponse<EmployeeInfoResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetBranchEmployeesQueryHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IBranchService _branchService;

    public GetBranchEmployeesQueryHandler(ILogger<GetBranchEmployeesQueryHandler> logger,
        IUserServiceApplicationDbContext dbContext, IBranchService branchService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _branchService = branchService;
    }

    public async Task<PagedResponse<EmployeeInfoResponseModel>> Handle(GetBranchEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all branch employees. PageNumber: {pageNumber}, PageSize: {pageSize}",
            request.PageNumber,
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
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"Company with Id {request.CompanyId} not found");
        }

        var branch = await _branchService.CheckBranchId(new BranchRequest
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            BranchId = request.BranchId,
            RequestedAt = DateTime.UtcNow
        });

        if (!branch.IsValid)
        {
            _logger.LogError("Branch with id {BranchId} not found", request.BranchId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"Branch with Id {request.BranchId} not found");
        }

        var totalCount = await _dbContext.Employees
            .Where(s => s.BranchId == request.BranchId && s.CompanyId == request.CompanyId)
            .CountAsync(cancellationToken);

        var dbEmployees = await _dbContext.Employees
            .Where(s => s.BranchId == request.BranchId && s.CompanyId == request.CompanyId)
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);


        var response = new List<EmployeeInfoResponseModel>();

        foreach (var employee in dbEmployees)
        {
            // var reviews = await _dbContext.Reviews
            //     .Include(s => s.Queue)
            //     .Where(s => s.Queue.EmployeeId == employee.Id)
            //     .ToListAsync(cancellationToken);

            double averageRating = 0;

            // if (reviews.Any())
            // {
            //     averageRating = reviews.Average(s => s.Grade);
            // }
            //
            // var complaints = await _dbContext.Complaints
            //     .Include(s => s.Queue)
            //     .Where(s => s.Queue.EmployeeId == employee.Id)
            //     .ToListAsync(cancellationToken);

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
                // TotalReviews = reviews.Count,
                AverageRating = averageRating,
                // TotalComplaints = complaints.Count
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