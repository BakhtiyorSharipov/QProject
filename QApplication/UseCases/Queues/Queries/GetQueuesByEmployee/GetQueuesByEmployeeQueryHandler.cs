using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Queues.Queries.GetQueuesByEmployee;

public class GetQueuesByEmployeeQueryHandler: IRequestHandler<GetQueuesByEmployeeQuery, List<QueueResponseModel>>
{
    private readonly ILogger<GetQueuesByEmployeeQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetQueuesByEmployeeQueryHandler(ILogger<GetQueuesByEmployeeQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<QueueResponseModel>> Handle(GetQueuesByEmployeeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting queues for EmployeeId: {EmployeeId}", request.EmployeeId);
        var dbQueue = await _dbContext.Queues.Where(s => s.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);
        if (!dbQueue.Any())
        {
            _logger.LogWarning("No queues found for EmployeeId: {EmployeeId}", request.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel()
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime ?? queue.StartTime.AddMinutes(30),
            Status = queue.Status
        }).ToList();

        _logger.LogInformation("Successfully fetched {QueueCount} queues for EmployeeId: {EmployeeId}", response.Count,
            request.EmployeeId);
        return response;
    }
}