using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Extensions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.Complaints.Commands.CreateComplaint;

public class CreateComplaintCommandHandler: IRequestHandler<CreateComplaintCommand, ComplaintResponseModel>
{
    private readonly ILogger<CreateComplaintCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public CreateComplaintCommandHandler(ILogger<CreateComplaintCommandHandler> logger, IQueueApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<ComplaintResponseModel> Handle(CreateComplaintCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding new complaint to this queue Id {queueId}", request.QueueId);

        var currentCustomer = await _contextAccessor.CurrentCustomer(_dbContext, cancellationToken);
        

        var queueId = await _dbContext.Queues
            .Where(s=>s.CustomerId== currentCustomer.Id)
            .FirstOrDefaultAsync(s => s.Id == request.QueueId, cancellationToken);
        if (queueId == null)
        {
            _logger.LogWarning("Queue with Id {queueId} not found for adding new complaint,", request.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }


        if (queueId.Status != QueueStatus.Completed && queueId.Status != QueueStatus.CanceledByAdmin &&
            queueId.Status != QueueStatus.CancelledByEmployee)
        {
            _logger.LogError("Invalid queue status while adding new complaint for this queue Id {queueId}",
                request.QueueId);
            throw new Exception("You can leave complaint when status is Completed or CanceledByAdmin/ByEmployee");
        }

        var complaints = await _dbContext.Complaints.Where(s => s.QueueId == request.QueueId)
            .ToListAsync(cancellationToken);
        var isDouble = complaints.Any(s => s.CustomerId == currentCustomer.Id);
        if (isDouble)
        {
            _logger.LogError("Overlapping complaint for this queue Id {queueId}", request.QueueId);
            throw new Exception("You have already left a complaint for this queue!");
        }

        var complaint = new ComplaintEntity
        {
            CustomerId = currentCustomer.Id,
            QueueId = request.QueueId,
            ComplaintText = request.ComplaintText,
            ComplaintStatus = ComplaintStatus.Pending,
            CreatdAt = DateTime.UtcNow
        };

        await _dbContext.Complaints.AddAsync(complaint, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Complaint added successfully with Id {id}.", complaint.Id);
        var response = new ComplaintResponseModel
        {
            Id = complaint.Id,
            CustomerId = complaint.CustomerId,
            QueueId = complaint.QueueId,
            EmployeeId = complaint.Queue.EmployeeId,
            ComplaintText = complaint.ComplaintText,
            ComplaintStatus = complaint.ComplaintStatus
        };

        return response;
    }
}