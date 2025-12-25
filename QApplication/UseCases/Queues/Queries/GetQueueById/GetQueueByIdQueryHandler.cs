using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Queues.Queries.GetQueueById;

public class GetQueueByIdQueryHandler: IRequestHandler<GetQueueByIdQuery, QueueResponseModel>
{
    private readonly ILogger<GetQueueByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetQueueByIdQueryHandler(ILogger<GetQueueByIdQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<QueueResponseModel> Handle(GetQueueByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting queue by Id {id}", request.Id);
        var dbQueue = await _dbContext.Queues.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbQueue == null)
        {
            _logger.LogWarning("Queue with Id {id} not found", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var response = new QueueResponseModel()
        {
            Id = dbQueue.Id,
            CustomerId = dbQueue.CustomerId,
            EmployeeId = dbQueue.EmployeeId,
            ServiceId = dbQueue.ServiceId,
            StartTime = dbQueue.StartTime,
            Status = dbQueue.Status
        };

        _logger.LogInformation("Queue with Id {id} fetched successfully", request.Id);

        return response;
    }
}