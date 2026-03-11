using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Exceptions;
using QApplication.Extensions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Queues.Queries.GetQueuesByEmployee;

public class GetQueuesByEmployeeQueryHandler: IRequestHandler<GetQueuesByEmployeeQuery, PagedResponse<QueueResponseModel>>
{
    private const int PageSize=15;
    private readonly ILogger<GetQueuesByEmployeeQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cacheService;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetQueuesByEmployeeQueryHandler(ILogger<GetQueuesByEmployeeQueryHandler> logger, IQueueApplicationDbContext dbContext, IHttpContextAccessor contextAccessor, ICacheService cacheService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
        _cacheService = cacheService;
    }

    public async Task<PagedResponse<QueueResponseModel>> Handle(GetQueuesByEmployeeQuery request, CancellationToken cancellationToken)
    {
        var currentEmployee = await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
        var employeeId = currentEmployee.Id;
        
        _logger.LogInformation("Getting all customer's queue. PageNumber: {pageNumber}, PageSize: {pageSize}",
            request.PageNumber, PageSize);

        var hashKey = CacheKeys.EmployeeQueuesHashKey(employeeId);
        var filed = CacheKeys.EmployeeQueuesField(request.PageNumber);

        var cached = await _cacheService.HashGetAsync<PagedResponse<QueueResponseModel>>(hashKey, filed);

        if (cached is not null)
        {
            return cached;
        }


        var query = _dbContext.Queues.Where(s => s.EmployeeId == employeeId);
        

        var totalCount = await query.CountAsync(cancellationToken);
        var queues = await query
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize).ToListAsync(cancellationToken);

        var response = queues.Select(queue => new QueueResponseModel()
        {
            Id = queue.Id,
            CompanyId = queue.CompanyId,
            BranchId = queue.BranchId,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime ?? queue.StartTime.AddMinutes(30),
            Status = queue.Status
        }).ToList();

        _logger.LogInformation("Successfully fetched {QueueCount} queues for EmployeeId: {EmployeeId}", response.Count,
            employeeId);

        var pagedResponse = new PagedResponse<QueueResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };
        
        return pagedResponse;
    }
}