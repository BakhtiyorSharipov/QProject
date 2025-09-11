using System.Net;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.QueueRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class QueueService: IQueueService
{
    private readonly IQueueRepository _repository;

    public QueueService(IQueueRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<QueueResponseModel> GetAll(int pageList, int pageNumber)
    {
        var dbQueue = _repository.GetAll(pageList, pageNumber);
        if (dbQueue==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var response = dbQueue.Select(queue => new QueueResponseModel()
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            DayOfWeek = queue.DayOfWeek,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime,
            CancelReason = queue.CancelReason
            
        }).ToList();

        return response;
    }

    public QueueResponseModel GetById(int id)
    {
        var dbQueue = _repository.FindById(id);
        if (dbQueue==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var response = new QueueResponseModel()
        {
            Id=dbQueue.Id,
            CustomerId = dbQueue.CustomerId,
            EmployeeId = dbQueue.EmployeeId,
            ServiceId = dbQueue.ServiceId,
            DayOfWeek = dbQueue.DayOfWeek,
            StartTime = dbQueue.StartTime,
            EndTime = dbQueue.EndTime,
            CancelReason = dbQueue.CancelReason
        };

        return response;
    }

    public QueueResponseModel Add(QueueRequestModel requestModel)
    {
        var requestToCreate = requestModel as CreateQueueRequest;
        if (requestToCreate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(QueueEntity));
        }

        var queue = new QueueEntity()
        {
            CustomerId = requestToCreate.CustomerId,
            EmployeeId = requestToCreate.EmployeeId,
            ServiceId = requestToCreate.ServiceId,
            DayOfWeek = requestToCreate.DayOfWeek,
            StartTime = requestToCreate.StartTime,
            EndTime = requestToCreate.EndTime,
            CancelReason = requestToCreate.CancelReason
        };
        
        _repository.Add(queue);
        _repository.SaveChanges();

        var response = new QueueResponseModel()
        {
            Id = queue.Id,
            CustomerId = queue.CustomerId,
            EmployeeId = queue.EmployeeId,
            ServiceId = queue.ServiceId,
            DayOfWeek = queue.DayOfWeek,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime,
            CancelReason = queue.CancelReason
        };

        return response;
    }

    public QueueResponseModel Update(int id, QueueRequestModel requestModel)
    {
        var dbQueue = _repository.FindById(id);
        if (dbQueue==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        var requestToUpdate = requestModel as UpdateQueueRequest;
        if (requestToUpdate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(QueueEntity));
        }

        dbQueue.CustomerId = requestToUpdate.CustomerId;
        dbQueue.EmployeeId = requestToUpdate.EmployeeId;
        dbQueue.ServiceId = requestToUpdate.ServiceId;
        dbQueue.DayOfWeek = requestToUpdate.DayOfWeek;
        dbQueue.StartTime = requestToUpdate.StartTime;
        dbQueue.EndTime = requestToUpdate.EndTime;
        dbQueue.CancelReason = requestToUpdate.CancelReason;
        
        _repository.Update(dbQueue);
        _repository.SaveChanges();

        var response = new QueueResponseModel()
        {
            Id = dbQueue.Id,
            CustomerId = dbQueue.CustomerId,
            EmployeeId = dbQueue.EmployeeId,
            ServiceId = dbQueue.ServiceId,
            DayOfWeek = dbQueue.DayOfWeek,
            StartTime = dbQueue.StartTime,
            EndTime = dbQueue.EndTime,
            CancelReason = dbQueue.CancelReason
        };

        return response;
    }

    public bool Delete(int id)
    {
        var dbQueue = _repository.FindById(id);
        if (dbQueue==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }
        
        _repository.Delete(dbQueue);
        _repository.SaveChanges();

        return true;
    }
    
}