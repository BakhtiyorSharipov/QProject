using System.Net;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.BlockedCustomerRequest;
using QApplication.Requests.QueueRequest;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Services;

public class QueueService : IQueueService
{
    private readonly IQueueRepository _repository;
    private readonly IAvailabilityScheduleRepository _scheduleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBlockedCustomerRepository _blockedCustomerRepository;
    private readonly ILogger<QueueService> _logger;

    public QueueService(IQueueRepository repository, IAvailabilityScheduleRepository scheduleRepository,
        ICustomerRepository customerRepository, IServiceRepository serviceRepository,
        IBlockedCustomerRepository blockedCustomerRepository, ILogger<QueueService> logger)
    {
        _repository = repository;
        _scheduleRepository = scheduleRepository;
        _customerRepository = customerRepository;
        _serviceRepository = serviceRepository;
        _blockedCustomerRepository = blockedCustomerRepository;
        _logger = logger;
    }

    public IEnumerable<QueueResponseModel> GetAll(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting all queues. PageNumber: {pageNumber}, PageList: {pageList}", pageNumber, pageList);
        var dbQueue = _repository.GetAll(pageList, pageNumber);
        if (dbQueue == null)
        {
            _logger.LogWarning("No queues found in system");
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel()
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            Status = queue.Status
        }).ToList();

        _logger.LogInformation("Fetched {queueCount} queues", response.Count);
        return response;
    }


    public QueueResponseModel GetById(int id)
    {
        _logger.LogInformation("Getting queue by Id {id}", id);
        var dbQueue = _repository.FindById(id);
        if (dbQueue == null)
        {
            _logger.LogWarning("Queue with Id {id} not found", id);
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
        
        _logger.LogInformation("Queue with Id {id} fetched successfully", id);

        return response;
    }

    public AddQueueResponseModel Add(QueueRequestModel requestModel)
    {
        _logger.LogInformation("Adding new queue for EmployeeId {id}", requestModel.EmployeeId);
        var requestToCreate = requestModel as CreateQueueRequest;
        if (requestToCreate == null)
        {
            _logger.LogError("Invalid request model while adding new queue");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(QueueEntity));
        }

        var schedule = _scheduleRepository.GetEmployeeById(requestToCreate.EmployeeId).ToList();
        if (!schedule.Any())
        {
            _logger.LogWarning("Employee with Id {id} not found in schedule entities for adding new queue", requestToCreate.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        var customer = _customerRepository.FindById(requestToCreate.CustomerId);
        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {id} not found for adding new queue ", requestToCreate.CustomerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var service = _serviceRepository.FindById(requestToCreate.ServiceId);
        if (service == null)
        {
            _logger.LogWarning("Service with Id {id} not found for adding new queue");
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        _logger.LogDebug("Checking if time slot {startTime} is available for 30- minute booking", requestToCreate.StartTime);
        var slotExists = schedule.Any(s => s.AvailableSlots.Any(slot =>
            requestToCreate.StartTime >= slot.From && requestToCreate.StartTime.AddMinutes(30) <= slot.To
        ));

        if (!slotExists)
        {
            _logger.LogWarning("Time slot {startTime} not available for EmployeeId: {id}", requestToCreate.StartTime, requestToCreate.EmployeeId);
            throw new Exception("The selected time slot is not available for a 30-minute booking. Please choose a start time that fits within the employee's working hours.");
        }


        
        _logger.LogDebug("Checking for overlapping queues for EmployeeId: {employeeId}", requestToCreate.EmployeeId);
        var allQueuesByEmployee = GetQueuesByEmployee(requestToCreate.EmployeeId)
            .Where(q=>q.Status == QueueStatus.Pending || q.Status == QueueStatus.Confirmed );

        var newQueueStart = requestToCreate.StartTime;
        var newQueueEnd = newQueueStart.AddMinutes(30);
        var isDouble = allQueuesByEmployee.Any(s =>
        {
            var existingStart = s.StartTime;
            var existingEnd = s.EndTime;
            return (newQueueStart < existingEnd && newQueueEnd > existingStart) &&
                   (s.Status == QueueStatus.Confirmed || s.Status == QueueStatus.Pending);
        }); 
        
        
        if (isDouble)
        {
            _logger.LogWarning("Time slot is already booked for employee Id {id}", requestToCreate.EmployeeId);
            throw new Exception("This slot is already booked!");
        }

        
        
        _logger.LogDebug("Checking if is customer blocked for CompanyId: {id}", service.CompanyId);
        var blocked = _blockedCustomerRepository.FindById(requestToCreate.CustomerId);
        if (blocked != null &&
            blocked.DoesBanForever &&
            service.CompanyId == blocked.CompanyId)
        {
            _logger.LogWarning("Customer {id} is blocked from Company {companyId}", requestToCreate.CustomerId, service.CompanyId);
            throw new Exception("You are blocked by this company!");
        }
        
     
        _logger.LogInformation("Creating new queue entity");
        var queue = new QueueEntity()
        {
            CustomerId = requestToCreate.CustomerId,
            EmployeeId = requestToCreate.EmployeeId,
            ServiceId = requestToCreate.ServiceId,
            StartTime = requestToCreate.StartTime,
            Status = QueueStatus.Pending
        };

        
        _repository.Add(queue);
        _logger.LogDebug("Saving new queue to repository");
        _repository.SaveChanges();

        
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


    public bool Delete(int id)
    {
        _logger.LogInformation("Deleting queue with Id: {id}", id);
        var dbQueue = _repository.FindById(id);
        if (dbQueue == null)
        {
            _logger.LogWarning("Queue with Id {id} not found for deleting", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        _repository.Delete(dbQueue);
        _repository.SaveChanges();
        _logger.LogInformation("Queue with Id {id} successfully deleted", id);
        return true;
    }


    public QueueResponseModel CancelQueueByCustomer(QueueCancelRequest request)
    {
        _logger.LogInformation("Cancelling queue Id {id} by customer", request.QueueId);
        var dbQueue = _repository.FindById(request.QueueId);
        if (dbQueue == null)
        {
            _logger.LogWarning("Queue with Id {QueueId} not found for customer cancellation", request.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }
        
        _logger.LogDebug("Current queue status: {Status}", dbQueue.Status);
        if (dbQueue.Status != QueueStatus.Pending && dbQueue.Status != QueueStatus.Confirmed)
        {
            _logger.LogWarning("Invalid queue status {Status} for customer cancellation", dbQueue.Status);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(QueueEntity));
        }

        _logger.LogDebug("Time until queue start: {minutes} minutes", (dbQueue.StartTime-DateTimeOffset.Now).TotalMinutes);
        if ((dbQueue.StartTime - DateTime.Now).TotalMinutes < 10)
        {
            _logger.LogWarning("Cancellation is than 10 minutes before start time");
            throw new Exception("Cannot cancel less than 10 minutes before start time");
        }

        dbQueue.Status = QueueStatus.CancelledByCustomer;
        dbQueue.CancelReason = request.CancelReason;

        _logger.LogDebug("Saving cancellation changes to repository");
        _repository.SaveChanges();

        var response = new QueueResponseModel
        {
            Id = dbQueue.Id,
            CustomerId = dbQueue.CustomerId,
            EmployeeId = dbQueue.EmployeeId,
            ServiceId = dbQueue.ServiceId,
            StartTime = dbQueue.StartTime,
            Status = dbQueue.Status
        };

        _logger.LogInformation("Successfully cancelled queue Id {id} by customer", request.QueueId);
        return response;
    }

    public QueueResponseModel CancelQueueByEmployee(QueueCancelRequest request)
    {
        _logger.LogInformation("Cancelling queue Id {id} by employee", request.QueueId);

        var dbQueue = _repository.FindById(request.QueueId);
        if (dbQueue == null)
        {
            _logger.LogWarning("Queue with Id {QueueId} not found for employee cancellation", request.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        }

        dbQueue.Status = QueueStatus.CancelledByEmployee;
        dbQueue.CancelReason = request.CancelReason;
        
        _logger.LogDebug("Saving employee cancellation changes to repository");
        _repository.SaveChanges();

        var response = new QueueResponseModel
        {
            Id = dbQueue.Id,
            CustomerId = dbQueue.CustomerId,
            EmployeeId = dbQueue.EmployeeId,
            ServiceId = dbQueue.ServiceId,
            StartTime = dbQueue.StartTime,
            Status = dbQueue.Status
        };
        
        _logger.LogInformation("Successfully cancelled queue Id {id} by employee", request.QueueId);
        return response;
    }

    public UpdateQueueStatusResponseModel UpdateQueueStatus(UpdateQueueRequest request)
    {
        _logger.LogInformation("Updating queue status for QueueId: {QueueId} to {NewStatus}", request.QueueId, request.newStatus);
        var dbQueue = _repository.FindById(request.QueueId);
        if (dbQueue == null)
        {
            _logger.LogWarning("Queue with Id {QueueId} not found for status update", request.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        _logger.LogDebug("Current queue status: {CurrentStatus}, requested new status: {NewStatus}", dbQueue.Status, request.newStatus);
        var employeeSchedule = _scheduleRepository.GetEmployeeById(dbQueue.EmployeeId).ToList();
        if (!employeeSchedule.Any())
        {
            _logger.LogWarning("Employee with Id {id} not found in schedule entities for adding new queue", dbQueue.EmployeeId);
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

        if (request.newStatus == QueueStatus.DidNotCome)
        {
            _logger.LogDebug("Checking DidNotCome count for CustomerId: {CustomerId}", dbQueue.CustomerId);
            var count = _repository.GetQueuesByCustomer(dbQueue.CustomerId)
                .Count(s => s.Status == QueueStatus.DidNotCome);

            if (count >= 3 && !_blockedCustomerRepository.Exists(dbQueue.CustomerId, dbQueue.Service.CompanyId))
            {
                _logger.LogWarning("CustomerId {id} automatically blocked for CompanyId {companyId}: 3+ DidNotCome", dbQueue.CustomerId, dbQueue.Service.CompanyId);
                BlockedCustomerEntity blockedCustomer = new BlockedCustomerEntity
                {
                    CustomerId = dbQueue.CustomerId,
                    CompanyId = dbQueue.Service.CompanyId,
                    DoesBanForever = true,
                    Reason = "Did not come 3 times",
                    BannedUntil = DateTime.MaxValue
                };

                _blockedCustomerRepository.Add(blockedCustomer);
                _blockedCustomerRepository.SaveChanges();
                throw new Exception("Customer has been automatically blocked due to multiple DidNotCome.");
            }
        }

        

        if (request.newStatus == QueueStatus.Confirmed )
        {
            DateTimeOffset startTimeUtc = dbQueue.StartTime.ToUniversalTime();
            DateTimeOffset endTimeUtc;
            if (request.EndTime.HasValue)
            {
                 endTimeUtc = request.EndTime.Value.ToUniversalTime();
                
                 _logger.LogDebug("Custom end time: {endTime} (UTC)", endTimeUtc);
                 
                if (endTimeUtc <= startTimeUtc)
                {
                    _logger.LogError("Invalid end time. Start: {StartTime} (UTC), End: {EndTime} (UTC)", startTimeUtc, endTimeUtc);
                    throw new Exception($"EndTime must be later than StartTime. " +
                                        $"Start: {startTimeUtc:dd.MM.yyyy HH:mm:ss} (UTC), " +
                                        $"End: {endTimeUtc:dd.MM.yyyy HH:mm:ss} (UTC)");
                }

                var slotExists = employeeSchedule.Any(s => s.AvailableSlots.Any(slot =>
                    startTimeUtc >= slot.From && endTimeUtc <= slot.To));

                if (!slotExists)
                {
                    _logger.LogWarning("Updated queue time outside employee working hours. Start: {StartTime}, End: {EndTime}", startTimeUtc, endTimeUtc);
                    throw new Exception("The updated queue time is outside the employee's working hours.");
                }

                var allQueueByEmployee = _repository.GetQueuesByEmployee(dbQueue.EmployeeId)
                    .Where(q => q.Status == QueueStatus.Pending || q.Status == QueueStatus.Confirmed)
                    .Where(q => q.Id != dbQueue.Id)
                    .ToList();

                var isOverlap = allQueueByEmployee.Any(s => startTimeUtc <(s.EndTime.HasValue? s.EndTime.Value: s.StartTime.AddMinutes(30)) && endTimeUtc > s.StartTime);

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
        }
        
        
        
        dbQueue.Status = request.newStatus;
        _logger.LogDebug("Saving status update to repository");
        _repository.SaveChanges();

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

        _logger.LogInformation("Successfully updated queue {QueueId} status to {NewStatus}", request.QueueId, request.newStatus);
        return response;
    }

    public IEnumerable<QueueResponseModel> GetQueuesByCustomer(int customerId)
    {
        _logger.LogInformation("Getting queues for CustomerId: {CustomerId}", customerId);
        var dbQueue = _repository.GetQueuesByCustomer(customerId);
        if (!dbQueue.Any())
        {
            _logger.LogWarning("No queues found for CustomerId: {CustomerId}", customerId);
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

        _logger.LogInformation("Successfully fetched {QueueCount} queues for CustomerId: {CustomerId}", response.Count, customerId);
        return response;
    }

    public IEnumerable<QueueResponseModel> GetQueuesByEmployee(int employeeId)
    {
        _logger.LogInformation("Getting queues for EmployeeId: {EmployeeId}", employeeId);
        var dbQueue = _repository.GetQueuesByEmployee(employeeId);
        if (!dbQueue.Any())
        {
            _logger.LogWarning("No queues found for EmployeeId: {EmployeeId}", employeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel()
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            EndTime= queue.EndTime ?? queue.StartTime.AddMinutes(30),
            Status = queue.Status
        }).ToList();

        _logger.LogInformation("Successfully fetched {QueueCount} queues for EmployeeId: {EmployeeId}", response.Count, employeeId);
        return response;
    }

    public IEnumerable<QueueResponseModel> GetQueuesByService(int serviceId)
    {
        _logger.LogInformation("Getting queues for ServiceId: {ServiceId}", serviceId);
        var dbQueue = _repository.GetQueuesByService(serviceId);
        if (!dbQueue.Any())
        {
            _logger.LogWarning("No queues found for ServiceId: {ServiceId}", serviceId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime ?? queue.StartTime.AddMinutes(30),
            Status = queue.Status
        }).ToList();

        _logger.LogInformation("Successfully fetched {QueueCount} queues for ServiceId: {ServiceId}", response.Count, serviceId);
        return response;
    }

    public IEnumerable<QueueResponseModel> GetQueuesByCompany(int companyId)
    {
        _logger.LogInformation("Getting queues for CompanyId: {CompanyId}", companyId);
        var dbQueue = _repository.GetQueuesByCompany(companyId);
        if (!dbQueue.Any())
        {
            _logger.LogWarning("No queues found for CompanyId: {CompanyId}", companyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime ?? queue.StartTime.AddMinutes(30),
            Status = queue.Status
        }).ToList();

        _logger.LogInformation("Successfully fetched {QueueCount} queues for CompanyId: {CompanyId}", response.Count, companyId);
        return response;
    }
}