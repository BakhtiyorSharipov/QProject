using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Queues.Queries;

public class GetAllQueuesQueryHandler: IRequestHandler<GetAllQueuesQuery, PagedResponse<QueueResponseModel>>
{
    private readonly ILogger<GetAllQueuesQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetAllQueuesQueryHandler(ILogger<GetAllQueuesQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<QueueResponseModel>> Handle(GetAllQueuesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all queues. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            request.PageSize);

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
    }
}