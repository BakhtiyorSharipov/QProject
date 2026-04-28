using System.Net;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Extensions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QBranchService.Contracts.Interfaces;
using QBranchService.Contracts.Requests;
using QContracts.Events;
using QContracts.QueueEvents.Enums;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.Queues.Commands.CreateQueue;

public class CreateQueueCommandHandler : IRequestHandler<CreateQueueCommand, AddQueueResponseModel>
{
    private readonly ILogger<CreateQueueCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IBranchService _branchService;

    public CreateQueueCommandHandler(ILogger<CreateQueueCommandHandler> logger, IQueueApplicationDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        IHttpContextAccessor contextAccessor,
        IBranchService branchService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _contextAccessor = contextAccessor;
        _branchService = branchService;
    }

    public async Task<AddQueueResponseModel> Handle(CreateQueueCommand request, CancellationToken cancellationToken)
    {
        var currentCustomer = await _contextAccessor.CurrentCustomer(_dbContext, cancellationToken);
        var customerId = currentCustomer.Id;

        _logger.LogInformation("Adding new queue for EmployeeId {id}", request.EmployeeId);


        var validationResponse = await _branchService.ValidateQueueCreationAsync(
            new QueueCreationValidationRequest
            {
                RequestId = Guid.NewGuid(),
                BranchId = request.BranchId,
                RequestedStartTime = request.StartTime
            });

        if (!validationResponse.IsValid)
        {
            _logger.LogWarning("Branch validation failed: {ErrorMessage}", validationResponse.ErrorMessage);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, validationResponse.ErrorMessage!);
        }

        
        var ticketsToday = await _dbContext.Queues
            .CountAsync(q => q.BranchId == request.BranchId &&
                             q.StartTime.Date == request.StartTime.Date &&
                             q.Status != QueueStatus.CancelledByEmployee &&
                             q.Status != QueueStatus.CancelledByCustomer,
                cancellationToken);

        if (ticketsToday >= validationResponse.MaxTicketsPerDay)
        {
            _logger.LogWarning("Daily ticket limit reached for Branch {BranchId}. Today: {TicketsToday}/{MaxTickets}",
                request.BranchId, ticketsToday, validationResponse.MaxTicketsPerDay);

            throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                $"Maximum tickets for today ({validationResponse.MaxTicketsPerDay}) has been reached");
        }

        _logger.LogInformation("Branch validation passed. Tickets today: {TicketsToday}/{MaxTickets}",
            ticketsToday, validationResponse.MaxTicketsPerDay);


        var companyResult = await _branchService.CheckCompanyId(new CompanyRequest
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            RequestedAt = DateTimeOffset.UtcNow
        });

        if (!companyResult.IsValid)
        {
            _logger.LogInformation("Company with Id {CompanyId} not found", request.CompanyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                companyResult.ErrorMessage ?? "Company not found");
        }

        var branchResult = await _branchService.CheckBranchId(new BranchRequest
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            BranchId = request.BranchId,
            RequestedAt = DateTimeOffset.UtcNow
        });

        if (!branchResult.IsValid)
        {
            _logger.LogInformation("Branch with Id {BranchId} not found", request.BranchId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                companyResult.ErrorMessage ?? "Branch not found");
        }

        var companyServiceResult = await _branchService.CheckCompanyServiceId(new CompanyServiceRequest
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            CompanyServiceId = request.ServiceId,
            RequestedAt = DateTimeOffset.UtcNow
        });

        if (!companyServiceResult.IsValid)
        {
            _logger.LogInformation("CompanyService with Id {CompanyServiceId} not found", request.ServiceId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                companyResult.ErrorMessage ?? "CompanyService not found");
        }

        var schedule = await _dbContext.AvailabilitySchedules.Where(s => s.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);
        if (!schedule.Any())
        {
            _logger.LogWarning("Employee with Id {id} not found in schedule entities for adding new queue",
                request.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        _logger.LogInformation(
            "IDs validated successfully for Company {CompanyId}, Branch {BranchId}, Service {ServiceId}",
            request.CompanyId, request.BranchId, request.ServiceId);


        var customer =
            await _dbContext.Customers.FirstOrDefaultAsync(s => s.Id == customerId, cancellationToken);
        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {id} not found for adding new queue ", customerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var employee = await _dbContext.Employees
            .Where(s => s.CompanyId == request.CompanyId)
            .FirstOrDefaultAsync(s => s.Id == request.EmployeeId, cancellationToken);
        if (employee == null)
        {
            _logger.LogWarning("Employee with Id {EmployeeId} not found for this company ", request.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"Employee with Id {request.EmployeeId} not found for this company");
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


        _logger.LogDebug("Checking if is customer blocked for CompanyId: {id}", request.CompanyId);
        var blocked =
            await _dbContext.BlockedCustomers.FirstOrDefaultAsync(s => s.CustomerId == customerId,
                cancellationToken);
        if (blocked != null &&
            blocked.DoesBanForever &&
            request.CompanyId == blocked.CompanyId)
        {
            _logger.LogWarning("Customer {id} is blocked from Company {companyId}", customerId,
                request.CompanyId);
            throw new Exception("You are blocked by this company!");
        }


        _logger.LogInformation("Creating new queue entity");
        var queue = new QueueEntity()
        {
            CompanyId = request.CompanyId,
            BranchId = request.BranchId,
            CustomerId = customerId,
            EmployeeId = request.EmployeeId,
            ServiceId = request.ServiceId,
            StartTime = request.StartTime,
            Status = QueueStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };


        await _dbContext.Queues.AddAsync(queue, cancellationToken);
        _logger.LogDebug("Saving new queue to repository");
        await _dbContext.SaveChangesAsync(cancellationToken);


        await _publishEndpoint.Publish(new QueueEvent
        {
            QueueId = queue.Id,
            CompanyId = queue.CompanyId,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime,
            EventType = QueueEventType.Created,
        }, cancellationToken);


        var response = new AddQueueResponseModel()
        {
            Id = queue.Id,
            CompanyId = queue.CompanyId,
            BranchId = queue.BranchId,
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