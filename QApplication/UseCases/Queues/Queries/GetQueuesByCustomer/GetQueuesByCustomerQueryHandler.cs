using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Queues.Queries.GetQueuesByCustomer;

public class GetQueuesByCustomerQueryHandler: IRequestHandler<GetQueuesByCustomerQuery, List<QueueResponseModel>>
{
    private readonly ILogger<GetQueuesByCustomerQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetQueuesByCustomerQueryHandler(ILogger<GetQueuesByCustomerQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<QueueResponseModel>> Handle(GetQueuesByCustomerQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting queues for CustomerId: {CustomerId}", request.CustomerId);
        var dbQueue = await _dbContext.Queues.Where(s => s.CustomerId == request.CustomerId)
            .ToListAsync(cancellationToken);
        if (!dbQueue.Any())
        {
            _logger.LogWarning("No queues found for CustomerId: {CustomerId}", request.CustomerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            Status = queue.Status
        }).ToList();

        _logger.LogInformation("Successfully fetched {QueueCount} queues for CustomerId: {CustomerId}", response.Count,
            request.CustomerId);
        return response;
    }
}