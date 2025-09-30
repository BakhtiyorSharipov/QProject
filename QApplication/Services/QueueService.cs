using System.Net;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.QueueRequest;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Services;

public class QueueService : IQueueService
{
    private readonly IQueueRepository _repository;

    public QueueService(IQueueRepository repository)
    {
        _repository = repository;
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

    public QueueResponseModel Add(QueueRequestModel requestModel)
    {
        var requestToCreate = requestModel as CreateQueueRequest;
        if (requestToCreate == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(QueueEntity));
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

        var response = new QueueResponseModel()
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


    public QueueResponseModel Update(int id, QueueRequestModel requestModel)
    {
        var dbQueue = _repository.FindById(id);
        if (dbQueue == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var requestToUpdate = requestModel as UpdateQueueRequest;
        if (requestToUpdate == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(QueueEntity));
        }

        dbQueue.CustomerId = requestToUpdate.CustomerId;
        dbQueue.EmployeeId = requestToUpdate.EmployeeId;
        dbQueue.ServiceId = requestToUpdate.ServiceId;
        dbQueue.StartTime = requestToUpdate.StartTime;

        _repository.Update(dbQueue);
        _repository.SaveChanges();

        var response = new QueueResponseModel()
        {
            Id = dbQueue.Id,
            CustomerId = dbQueue.CustomerId,
            EmployeeId = dbQueue.EmployeeId,
            ServiceId = dbQueue.ServiceId,
            StartTime = dbQueue.StartTime,
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

    public QueueResponseModel UpdateQueueStatus(int id, QueueStatus newStatus)
    {
        var dbQueue = _repository.FindById(id);
        if (dbQueue == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        switch (dbQueue.Status)
        {
            case QueueStatus.Pending:
                if (newStatus != QueueStatus.Confirmed && newStatus != QueueStatus.CancelledByEmployee)
                {
                    throw new Exception("Pending queue can only be Confirmed or Cancelled!");
                }

                break;
            case QueueStatus.Confirmed:
                if (newStatus != QueueStatus.Completed && newStatus != QueueStatus.DidNotCome &&
                    newStatus != QueueStatus.CancelledByEmployee)
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

        if (newStatus != QueueStatus.Completed && newStatus != QueueStatus.DidNotCome &&
            newStatus != QueueStatus.Confirmed)
        {
            throw new Exception("Invalid status update by employee");
        }

        dbQueue.Status = newStatus;
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

    public IEnumerable<QueueResponseModel> GetQueuesByCustomer(int customerId)
    {
        var dbQueue = _repository.GetQueuesByCustomer(customerId);
        if (dbQueue == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
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
        if (dbQueue == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel()
        {
            Id = queue.Id,
            CustomerId = queue.Id,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            StartTime = queue.StartTime,
            Status = queue.Status
        }).ToList();

        return response;
    }
}