using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using StackExchange.Redis;

namespace QApplication.UseCases.Queues.Queries;

public class GetAllQueuesQueryHandler: IRequestHandler<GetAllQueuesQuery, PagedResponse<QueueResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllQueuesQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;
    public GetAllQueuesQueryHandler(ILogger<GetAllQueuesQueryHandler> logger, IQueueApplicationDbContext dbContext, ICacheService cache)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<PagedResponse<QueueResponseModel>> Handle(GetAllQueuesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all queues. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber, PageSize);

        var hashKey = CacheKeys.AllQueuesHashKey;
        var filed = CacheKeys.AllQueuesField(request.PageNumber );

        var cached = await _cache.HashGetAsync<PagedResponse<QueueResponseModel>>(hashKey, filed, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }
        
        var totalCount = await _dbContext.Queues.CountAsync(cancellationToken);

        var dbQueues =await  _dbContext.Queues
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);
                
                
        var response = dbQueues.Select(queue => new QueueResponseModel()
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            Status = queue.Status
        }).ToList();

                
                
        _logger.LogInformation("Fetched {queueCount} queues.", response.Count);

        var pagedResponse =new PagedResponse<QueueResponseModel>
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