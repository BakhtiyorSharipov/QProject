using System.Net;
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

public class GetQueuesByCustomerQueryHandler : IRequestHandler<GetQueuesByCustomerQuery, PagedResponse<QueueResponseModel>>
{
    private const int PageSize = 15;
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
            request.PageNumber, PageSize);

       

        var hashKey = CacheKeys.CustomerQueuesHashKey(userId);
        var filed = CacheKeys.CustomerQueuesField(request.PageNumber);

        var cached = await _cache.HashGetAsync<PagedResponse<QueueResponseModel>>(hashKey, filed, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }
        
        var query = _dbContext.Queues
            .Where(s => s.CustomerId == customerId);

        var totalCount = await query.CountAsync(cancellationToken);

        var queues = await query
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Skip((request.PageNumber - 1) * 15)
            .Take(15).ToListAsync(cancellationToken);


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
        
                
        _logger.LogInformation("Successfully fetched {QueueCount} queues for CustomerId: {CustomerId}",
            response.Count,
            customerId);
        
        
        _logger.LogInformation("Fetched {companyCount} companies.", response.Count);
        var pagedResponse=new PagedResponse<QueueResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };

        await _cache.HashSetAsync(hashKey, filed, pagedResponse, TimeSpan.FromMinutes(10), cancellationToken);

        return pagedResponse;
        
    }
}