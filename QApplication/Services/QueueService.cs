using System.Net;
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

    public QueueService(IQueueRepository repository, IAvailabilityScheduleRepository scheduleRepository,
        ICustomerRepository customerRepository, IServiceRepository serviceRepository,
        IBlockedCustomerRepository blockedCustomerRepository)
    {
        _repository = repository;
        _scheduleRepository = scheduleRepository;
        _customerRepository = customerRepository;
        _serviceRepository = serviceRepository;
        _blockedCustomerRepository = blockedCustomerRepository;
    }

    public IEnumerable<QueueResponseModel> GetAll(int pageList, int pageNumber)
    {
        var dbQueue = _repository.GetAll(pageList, pageNumber);
        if (dbQueue == null)
        {
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

        return response;
    }


    public QueueResponseModel GetById(int id)
    {
        var dbQueue = _repository.FindById(id);
        if (dbQueue == null)
        {
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

        return response;
    }

    public AddQueueResponseModel Add(QueueRequestModel requestModel)
    {
        var requestToCreate = requestModel as CreateQueueRequest;
        if (requestToCreate == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(QueueEntity));
        }

        var schedule = _scheduleRepository.GetEmployeeById(requestToCreate.EmployeeId);
        if (!schedule.Any())
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        var customer = _customerRepository.FindById(requestToCreate.CustomerId);
        if (customer == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var service = _serviceRepository.FindById(requestToCreate.ServiceId);
        if (service == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var dayOfWeek = schedule.Where(s => s.DayOfWeek == requestToCreate.StartTime.DayOfWeek).ToList();

        if (!dayOfWeek.Any())
        {
            throw new Exception("No schedule found for this day!");
        }

        var slotExists = dayOfWeek.Any(s => s.AvailableSlots.Any(slot =>
            requestToCreate.StartTime >= slot.From && requestToCreate.StartTime.AddHours(1) < slot.To
        ));

        if (!slotExists)
        {
            throw new Exception("The selected time slot is not available for a 1-hour booking. Please choose a start time that fits within the employee's working hours.");
        }


        var allQueuesByEmployee = GetQueuesByEmployee(requestToCreate.EmployeeId)
            .Where(q=>q.Status == QueueStatus.Pending || q.Status == QueueStatus.Confirmed );

        var newQueueStart = requestToCreate.StartTime;
        var newQueueEnd = newQueueStart.AddHours(1);
        var isDouble = allQueuesByEmployee.Any(s =>
        {
            var existingStart = s.StartTime;
            var existingEnd = s.EndTime;
            return (newQueueStart < existingEnd && newQueueEnd > existingStart) &&
                   (s.Status == QueueStatus.Confirmed);
        }); 
        
        
        if (isDouble)
        {
            throw new Exception("This slot is already booked!");
        }

        
        
        var blocked = _blockedCustomerRepository.FindById(requestToCreate.CustomerId);
        if (blocked != null &&
            blocked.DoesBanForever &&
            service.CompanyId == blocked.CompanyId)
        {
            throw new Exception("You are blocked by this company!");
        }

        
        
        
        var queue = new QueueEntity()
        {
            CustomerId = requestToCreate.CustomerId,
            EmployeeId = requestToCreate.EmployeeId,
            ServiceId = requestToCreate.ServiceId,
            StartTime = requestToCreate.StartTime,
            Status = QueueStatus.Pending
        };

        _repository.Add(queue);
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

        return response;
    }


    public bool Delete(int id)
    {
        var dbQueue = _repository.FindById(id);
        if (dbQueue == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        _repository.Delete(dbQueue);
        _repository.SaveChanges();

        return true;
    }


    public QueueResponseModel CancelQueueByCustomer(QueueCancelRequest request)
    {
        var dbQueue = _repository.FindById(request.QueueId);
        if (dbQueue == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        if (dbQueue.Status != QueueStatus.Pending && dbQueue.Status != QueueStatus.Confirmed)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(QueueEntity));
        }

        if ((dbQueue.StartTime - DateTime.Now).TotalMinutes < 10)
        {
            throw new Exception("Cannot cancel less than 10 minutes before start time");
        }

        dbQueue.Status = QueueStatus.CancelledByCustomer;
        dbQueue.CancelReason = request.CancelReason;

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

        return response;
    }

    public QueueResponseModel CancelQueueByEmployee(QueueCancelRequest request)
    {
        var dbQueue = _repository.FindById(request.QueueId);
        if (dbQueue == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        }

        dbQueue.Status = QueueStatus.CancelledByEmployee;
        dbQueue.CancelReason = request.CancelReason;

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

        return response;
    }

    public UpdateQueueStatusResponseModel UpdateQueueStatus(UpdateQueueRequest request)
    {
        var dbQueue = _repository.FindById(request.QueueId);
        if (dbQueue == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        switch (dbQueue.Status)
        {
            case QueueStatus.Pending:
                if (request.newStatus != QueueStatus.Confirmed && request.newStatus != QueueStatus.CancelledByEmployee)
                {
                    throw new Exception("Pending queue can only be Confirmed or Cancelled!");
                }

                break;
            case QueueStatus.Confirmed:
                if (request.newStatus != QueueStatus.Completed && request.newStatus != QueueStatus.DidNotCome &&
                    request.newStatus != QueueStatus.CancelledByEmployee)
                {
                    throw new Exception("Confirmed queues can only be Completed, DidNotCome, Cancelled!");
                }

                break;
            case QueueStatus.Completed:
            case QueueStatus.CancelledByEmployee:
            case QueueStatus.CancelledByCustomer:
            case QueueStatus.CanceledByAdmin:
            case QueueStatus.DidNotCome:
                throw new Exception("This queue is already finalized and cannot be updated!");
        }

        if (request.newStatus != QueueStatus.Completed && request.newStatus != QueueStatus.DidNotCome &&
            request.newStatus != QueueStatus.Confirmed)
        {
            throw new Exception("Invalid status update by employee");
        }


        var count = _repository.GetQueuesByCustomer(dbQueue.CustomerId)
            .Count(s => s.Status == QueueStatus.DidNotCome);

        if (count >= 3)

        {
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

        if (request.newStatus == QueueStatus.Confirmed )
        {
            dbQueue.EndTime = dbQueue.StartTime.AddHours(1);
        }
        
        dbQueue.Status = request.newStatus;
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

        return response;
    }

    public IEnumerable<QueueResponseModel> GetQueuesByCustomer(int customerId)
    {
        var dbQueue = _repository.GetQueuesByCustomer(customerId);
        if (!dbQueue.Any())
        {
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

        return response;
    }

    public IEnumerable<QueueResponseModel> GetQueuesByEmployee(int employeeId)
    {
        var dbQueue = _repository.GetQueuesByEmployee(employeeId);
        if (!dbQueue.Any())
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel()
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            EndTime= queue.EndTime ?? queue.StartTime.AddHours(1),
            Status = queue.Status
        }).ToList();

        return response;
    }

    public IEnumerable<QueueResponseModel> GetQueuesByService(int serviceId)
    {
        var dbQueue = _repository.GetQueuesByService(serviceId);
        if (!dbQueue.Any())
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime ?? queue.StartTime.AddHours(1),
            Status = queue.Status
        }).ToList();

        return response;
    }

    public IEnumerable<QueueResponseModel> GetQueuesByCompany(int companyId)
    {
        var dbQueue = _repository.GetQueuesByCompany(companyId);
        if (!dbQueue.Any())
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime ?? queue.StartTime.AddHours(1),
            Status = queue.Status
        }).ToList();

        return response;
    }
}