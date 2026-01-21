using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Queues.Queries;

public class GetAllQueuesQueryHandler: IRequestHandler<GetAllQueuesQuery, PagedResponse<QueueResponseModel>>
{
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
        _logger.LogInformation("Getting all queues. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            request.PageSize);

        var keyCache = CacheKeys.AllQueues(request.PageNumber, request.PageSize);

        var queues = await _cache.GetOrCreateAsync(keyCache, async () =>
            {
                
                var totalCount = await _dbContext.Queues.CountAsync(cancellationToken);

                var dbQueues =await  _dbContext.Queues
                    .OrderBy(s => s.Id)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
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

                return new PagedResponse<QueueResponseModel>
                {
                    Items = response,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };
                

            }, absoluteExpiration: TimeSpan.FromMinutes(10), slidingExpiration: TimeSpan.FromMinutes(5),
            cancellationToken: cancellationToken);

        if (queues==null)
        {
            _logger.LogInformation("Data is null. Can not return!");
            throw new Exception("Retrieved data is null");
        }
        
        return queues;
    }
}