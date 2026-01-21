using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Queues.Queries.GetQueuesByCustomer;

public class
    GetQueuesByCustomerQueryHandler : IRequestHandler<GetQueuesByCustomerQuery, PagedResponse<QueueResponseModel>>
{
    private readonly ILogger<GetQueuesByCustomerQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetQueuesByCustomerQueryHandler(ILogger<GetQueuesByCustomerQueryHandler> logger,
        IQueueApplicationDbContext dbContext, ICacheService cache, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
        _contextAccessor = contextAccessor;
    }

    public async Task<PagedResponse<QueueResponseModel>> Handle(GetQueuesByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var userClaim = _contextAccessor.HttpContext!.User;

        var userIdClaim = userClaim.FindFirst("id");
        
        
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var users = await _dbContext.Users.Where(s => s.Id == userId).ToListAsync(cancellationToken);

        int customerId=0;
        foreach (var user in users)
        {
            if (user.CustomerId.HasValue)
            {
                customerId = user.CustomerId.Value;
            }
            else
            {
                _logger.LogInformation("Customer not found");
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
            }
        }
        

        _logger.LogInformation("Getting all customer's queue. PageNumber: {pageNumber}, PageSize: {pageSize}",
            request.pageNumber,
            request.pageSize);

        var keyCache = CacheKeys.CustomerQueues(userId, request.pageNumber, request.pageSize);

        var queues = await _cache.GetOrCreateAsync(keyCache, async () =>
            {

                var query = _dbContext.Queues
                    .Where(s => s.CustomerId == customerId);

                var totalCount = await query.CountAsync(cancellationToken);

                var queues = await query
                    .AsNoTracking()
                    .OrderBy(c => c.Id)
                    .Skip((request.pageNumber - 1) * request.pageSize)
                    .Take(request.pageSize).ToListAsync(cancellationToken);


                var response = queues.Select(queue => new QueueResponseModel()
                {
                    Id = queue.Id,
                    CustomerId = queue.CustomerId,
                    EmployeeId = queue.EmployeeId,
                    ServiceId = queue.ServiceId,
                    StartTime = queue.StartTime,
                    EndTime = queue.EndTime ?? queue.StartTime.AddMinutes(30),
                    Status = queue.Status
                }).ToList();

                _logger.LogInformation("Successfully fetched {QueueCount} queues for EmployeeId: {EmployeeId}",
                    response.Count,
                    customerId);
                _logger.LogInformation("Fetched {companyCount} companies.", response.Count);
                return new PagedResponse<QueueResponseModel>
                {
                    Items = response,
                    PageNumber = request.pageNumber,
                    PageSize = request.pageSize,
                    TotalCount = totalCount
                };
            }, absoluteExpiration: TimeSpan.FromMinutes(5), slidingExpiration: TimeSpan.FromMinutes(5),
            cancellationToken: cancellationToken);

        if (queues == null)
        {
            _logger.LogInformation("Data is null. Can not return!");
            throw new Exception("Retrieved data is null");
        }

        return queues;
    }
}