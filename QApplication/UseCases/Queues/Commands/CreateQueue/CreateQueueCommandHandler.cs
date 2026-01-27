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

namespace QApplication.UseCases.Queues.Commands.CreateQueue;

public class CreateQueueCommandHandler: IRequestHandler<CreateQueueCommand, AddQueueResponseModel>
{
    private readonly ILogger<CreateQueueCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;
    public CreateQueueCommandHandler(ILogger<CreateQueueCommandHandler> logger, IQueueApplicationDbContext dbContext, ICacheService cache, IMediator mediator)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
        _mediator = mediator;
    }
    
    public async Task<AddQueueResponseModel> Handle(CreateQueueCommand request, CancellationToken cancellationToken)
    {
         _logger.LogInformation("Adding new queue for EmployeeId {id}", request.EmployeeId);
        

        var schedule = await _dbContext.AvailabilitySchedules.Where(s => s.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);
        if (!schedule.Any())
        {
            _logger.LogWarning("Employee with Id {id} not found in schedule entities for adding new queue",
                request.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        var customer =
            await _dbContext.Customers.FirstOrDefaultAsync(s => s.Id == request.CustomerId, cancellationToken);
        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {id} not found for adding new queue ", request.CustomerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var service = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == request.ServiceId, cancellationToken);
        if (service == null)
        {
            _logger.LogWarning("Service with Id {id} not found for adding new queue", service.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        _logger.LogDebug("Checking if time slot {startTime} is available for 30- minute booking",
            request.StartTime);
        var slotExists = schedule.Any(s => s.AvailableSlots.Any(slot =>
            request.StartTime >= slot.From && request.StartTime.AddMinutes(30) <= slot.To
        ));

        if (!slotExists)
        {
            _logger.LogWarning("Time slot {startTime} not available for EmployeeId: {id}", request.StartTime,
                request.EmployeeId);
            throw new Exception(
                "The selected time slot is not available for a 30-minute booking. Please choose a start time that fits within the employee's working hours.");
        }


        _logger.LogDebug("Checking for overlapping queues for EmployeeId: {employeeId}", request.EmployeeId);
        
        var allQueuesByEmployee = await _dbContext.Queues.Where(s => s.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);
        
        var allQueuesByEmployeeAfterFilter =
            allQueuesByEmployee.Where(q => q.Status == QueueStatus.Pending || q.Status == QueueStatus.Confirmed);

        var newQueueStart = request.StartTime;
        var newQueueEnd = newQueueStart.AddMinutes(30);
        var isDouble = allQueuesByEmployeeAfterFilter.Any(s =>
        {
            var existingStart = s.StartTime;
            var existingEnd = s.EndTime;
            return (newQueueStart < existingEnd && newQueueEnd > existingStart) &&
                   (s.Status == QueueStatus.Confirmed || s.Status == QueueStatus.Pending);
        });


        if (isDouble)
        {
            _logger.LogWarning("Time slot is already booked for employee Id {id}", request.EmployeeId);
            throw new Exception("This slot is already booked!");
        }


        _logger.LogDebug("Checking if is customer blocked for CompanyId: {id}", service.CompanyId);
        var blocked =
            await _dbContext.BlockedCustomers.FirstOrDefaultAsync(s => s.CustomerId == request.CustomerId,
                cancellationToken);
        if (blocked != null &&
            blocked.DoesBanForever &&
            service.CompanyId == blocked.CompanyId)
        {
            _logger.LogWarning("Customer {id} is blocked from Company {companyId}", request.CustomerId,
                service.CompanyId);
            throw new Exception("You are blocked by this company!");
        }


        _logger.LogInformation("Creating new queue entity");
        var queue = new QueueEntity()
        {
            CustomerId = request.CustomerId,
            EmployeeId = request.EmployeeId,
            ServiceId = request.ServiceId,
            StartTime = request.StartTime,
            Status = QueueStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        
        queue.Book();

        await _dbContext.Queues.AddAsync(queue, cancellationToken);
        _logger.LogDebug("Saving new queue to repository");
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(CacheKeys.AllQueues(1, 10), cancellationToken);
        var events = queue.DomainEvents.ToList();
        queue.ClearDomainEvents();
        foreach (var domainEvent in events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
        
        var response = new AddQueueResponseModel()
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            Status = queue.Status
        };

        _logger.LogInformation("Successfully added new queue with Id: {id}", queue.Id);
        return response;
    }
}