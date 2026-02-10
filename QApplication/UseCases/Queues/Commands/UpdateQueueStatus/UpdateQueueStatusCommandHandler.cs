using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;
using StackExchange.Redis;

namespace QApplication.UseCases.Queues.Commands.UpdateQueueStatus;

public class UpdateQueueStatusCommandHandler: IRequestHandler<UpdateQueueStatusCommand, UpdateQueueStatusResponseModel>
{
    private readonly ILogger<UpdateQueueStatusCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;

    public UpdateQueueStatusCommandHandler(ILogger<UpdateQueueStatusCommandHandler> logger, IQueueApplicationDbContext dbContext, ICacheService cache, IMediator mediator)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<UpdateQueueStatusResponseModel> Handle(UpdateQueueStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating queue status for QueueId: {QueueId} to {NewStatus}", request.QueueId,
            request.newStatus);
        var dbQueue = await _dbContext.Queues.FirstOrDefaultAsync(s => s.Id == request.QueueId, cancellationToken);
        if (dbQueue == null)
        {
            _logger.LogWarning("Queue with Id {QueueId} not found for status update", request.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        _logger.LogDebug("Current queue status: {CurrentStatus}, requested new status: {NewStatus}", dbQueue.Status,
            request.newStatus);
        var employeeSchedule = await _dbContext.AvailabilitySchedules.Where(s => s.EmployeeId == dbQueue.EmployeeId)
            .ToListAsync(cancellationToken);

        if (!employeeSchedule.Any())
        {
            _logger.LogWarning("Employee with Id {id} not found in schedule entities for adding new queue",
                dbQueue.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }

        switch (dbQueue.Status)
        {
            case QueueStatus.Pending:
                if (request.newStatus != QueueStatus.Confirmed && request.newStatus != QueueStatus.CancelledByEmployee)
                {
                    _logger.LogWarning("Invalid Pending status updating for this queue Id {id}", request.QueueId);
                    throw new Exception("Pending queue can only be Confirmed or Cancelled!");
                }

                break;
            case QueueStatus.Confirmed:
                if (request.newStatus != QueueStatus.Completed && request.newStatus != QueueStatus.DidNotCome &&
                    request.newStatus != QueueStatus.CancelledByEmployee)
                {
                    _logger.LogWarning("Invalid Confirmed status updating for this queue Id {id}", request.QueueId);
                    throw new Exception("Confirmed queues can only be Completed, DidNotCome, Cancelled!");
                }

                break;
            case QueueStatus.Completed:
            case QueueStatus.CancelledByEmployee:
            case QueueStatus.CancelledByCustomer:
            case QueueStatus.CanceledByAdmin:
            case QueueStatus.DidNotCome:
                _logger.LogWarning("Invalid finalized status updating for this queue Id {id}", request.QueueId);
                throw new Exception("This queue is already finalized and cannot be updated!");
        }

        if (request.newStatus != QueueStatus.Completed && request.newStatus != QueueStatus.DidNotCome &&
            request.newStatus != QueueStatus.Confirmed)
        {
            _logger.LogWarning("Invalid status update by employee: {newStatus}", request.newStatus);
            throw new Exception("Invalid status update by employee");
        }

        bool Exists(int customerId, int companyId)
        {
            var customer = _dbContext.BlockedCustomers.Where(s => s.CustomerId == customerId);
            var company = _dbContext.BlockedCustomers.Where(s => s.CompanyId == companyId);

            if (customer.Any() && company.Any())
            {
                return true;
            }

            return false;
        }
        
        if (request.newStatus == QueueStatus.DidNotCome)
        {
            _logger.LogDebug("Checking DidNotCome count for CustomerId: {CustomerId}", dbQueue.CustomerId);

            var queuesByCustomer = await _dbContext.Queues.Where(s => s.CustomerId == dbQueue.CustomerId)
                .ToListAsync(cancellationToken);
            
            var count = queuesByCustomer.Count(s => s.Status == QueueStatus.DidNotCome);
            if (count >= 3 && !Exists(dbQueue.CustomerId, dbQueue.Service.CompanyId))
            {
                _logger.LogWarning("CustomerId {id} automatically blocked for CompanyId {companyId}: 3+ DidNotCome",
                    dbQueue.CustomerId, dbQueue.Service.CompanyId);
                BlockedCustomerEntity blockedCustomer = new BlockedCustomerEntity
                {
                    CustomerId = dbQueue.CustomerId,
                    CompanyId = dbQueue.Service.CompanyId,
                    DoesBanForever = true,
                    Reason = "Did not come 3 times",
                    BannedUntil = DateTime.MaxValue,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.BlockedCustomers.AddAsync(blockedCustomer, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                throw new Exception("Customer has been automatically blocked due to multiple DidNotCome.");
            }
        }


        if (request.newStatus == QueueStatus.Confirmed)
        {
            DateTimeOffset startTimeUtc = dbQueue.StartTime.ToUniversalTime();
            DateTimeOffset endTimeUtc;
            if (request.EndTime.HasValue)
            {
                endTimeUtc = request.EndTime.Value.ToUniversalTime();

                _logger.LogDebug("Custom end time: {endTime} (UTC)", endTimeUtc);

                if (endTimeUtc <= startTimeUtc)
                {
                    _logger.LogError("Invalid end time. Start: {StartTime} (UTC), End: {EndTime} (UTC)", startTimeUtc,
                        endTimeUtc);
                    throw new Exception($"EndTime must be later than StartTime. " +
                                        $"Start: {startTimeUtc:dd.MM.yyyy HH:mm:ss} (UTC), " +
                                        $"End: {endTimeUtc:dd.MM.yyyy HH:mm:ss} (UTC)");
                }

                var slotExists = employeeSchedule.Any(s => s.AvailableSlots.Any(slot =>
                    startTimeUtc >= slot.From && endTimeUtc <= slot.To));

                if (!slotExists)
                {
                    _logger.LogWarning(
                        "Updated queue time outside employee working hours. Start: {StartTime}, End: {EndTime}",
                        startTimeUtc, endTimeUtc);
                    throw new Exception("The updated queue time is outside the employee's working hours.");
                }

                var queuesByEmployee =  _dbContext.Queues.Where(s => s.EmployeeId == dbQueue.EmployeeId);
                
                var allQueueByEmployee = await queuesByEmployee
                    .Where(q => q.Status == QueueStatus.Pending || q.Status == QueueStatus.Confirmed)
                    .Where(q => q.Id != dbQueue.Id)
                    .ToListAsync(cancellationToken);

                var isOverlap = allQueueByEmployee.Any(s =>
                    startTimeUtc < (s.EndTime.HasValue ? s.EndTime.Value : s.StartTime.AddMinutes(30)) &&
                    endTimeUtc > s.StartTime);

                if (isOverlap)
                {
                    _logger.LogWarning("Time overlap detected for EmployeeId: {EmployeeId}", dbQueue.EmployeeId);
                    throw new Exception("The updated queue time overlaps with another existing queue.");
                }

                dbQueue.EndTime = endTimeUtc;
                _logger.LogDebug("Set custom end time: {EndTime} (UTC)", endTimeUtc);
            }
            else
            {
                dbQueue.EndTime = dbQueue.StartTime.AddMinutes(30);
                _logger.LogDebug("Set default end time (30 minutes): {EndTime} (UTC)", dbQueue.EndTime);
            }
            
            dbQueue.Confirm();
        }


        if (request.newStatus== QueueStatus.Completed)
        {
            dbQueue.Complete();
        }

        dbQueue.Status = request.newStatus;
        _logger.LogDebug("Saving status update to repository");
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _cache.HashRemoveAsync(CacheKeys.AllQueuesHashKey, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.QueueId(request.QueueId), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.CustomerQueuesHashKey(dbQueue.CustomerId), cancellationToken);
        
        
        var events = dbQueue.DomainEvents.ToList();
        dbQueue.ClearDomainEvents();

        foreach (var domainEvent in events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
        
        
        var response = new UpdateQueueStatusResponseModel
        {
            Id = dbQueue.Id,
            CustomerId = dbQueue.CustomerId,
            EmployeeId = dbQueue.EmployeeId,
            ServiceId = dbQueue.ServiceId,
            StartTime = dbQueue.StartTime,
            EndTime = dbQueue.EndTime.Value,
            Status = dbQueue.Status
        };

        _logger.LogInformation("Successfully updated queue {QueueId} status to {NewStatus}", request.QueueId,
            request.newStatus);
        return response;
    }
}