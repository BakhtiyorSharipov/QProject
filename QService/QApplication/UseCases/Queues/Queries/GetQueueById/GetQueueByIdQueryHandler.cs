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

namespace QApplication.UseCases.Queues.Queries.GetQueueById;

public class GetQueueByIdQueryHandler: IRequestHandler<GetQueueByIdQuery, QueueResponseModel>
{
    private readonly ILogger<GetQueueByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetQueueByIdQueryHandler(ILogger<GetQueueByIdQueryHandler> logger, IQueueApplicationDbContext dbContext, ICacheService cache, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
        _contextAccessor = contextAccessor;
    }

    public async Task<QueueResponseModel> Handle(GetQueueByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting queue by Id {id}", request.Id);

        var isUserEmployee = await _contextAccessor.IsEmployee(_dbContext, cancellationToken);
        int customerId = 0;
        int employeeId = 0;
        if (isUserEmployee)
        {
            var currentEmployee = await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
            employeeId = currentEmployee.Id;
        }
        else
        {
            var currentCustomer = await _contextAccessor.CurrentCustomer(_dbContext, cancellationToken);
            customerId = currentCustomer.Id;
        }
        
        
        var keyCache = CacheKeys.QueueId(request.Id);
        var queue = await _cache.GetOrCreateAsync(keyCache, async () =>
        {
            _logger.LogInformation($"Cache miss for QueueId: {request.Id}");
            var dbQueue = await _dbContext.Queues
                .Where(s=>isUserEmployee
                ? s.EmployeeId== employeeId
                : s.CustomerId== customerId)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (dbQueue == null)
            {
                _logger.LogWarning("Queue with Id {id} not found", request.Id);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
            }

            var response = new QueueResponseModel()
            {
                Id = dbQueue.Id,
                CompanyId = dbQueue.CompanyId,
                BranchId = dbQueue.BranchId,
                CustomerId = dbQueue.CustomerId,
                EmployeeId = dbQueue.EmployeeId,
                ServiceId = dbQueue.ServiceId,
                StartTime = dbQueue.StartTime,
                Status = dbQueue.Status
            };
            

            _logger.LogInformation("Queue with Id {id} fetched successfully", request.Id);
            return response;

        }, absoluteExpiration: TimeSpan.FromMinutes(10), slidingExpiration: TimeSpan.FromMinutes(5));

        if (queue==null)
        {
            _logger.LogInformation("Data is null. Can not return!");
            throw new Exception("Retrieved data is null");
        }

        return queue;
    }
}