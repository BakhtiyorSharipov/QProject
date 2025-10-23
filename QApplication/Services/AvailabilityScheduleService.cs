using System.Net;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.AvailabilityScheduleRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Services;

public class AvailabilityScheduleService: IAvailabilityScheduleService
{
    private readonly IAvailabilityScheduleRepository _repository;

    public AvailabilityScheduleService(IAvailabilityScheduleRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<AvailabilityScheduleResponseModel> GetAll(int pageList, int pageNumber)
    {
        var dbAvailabilitySchedule = _repository.GetAll(pageList, pageNumber);
        if (dbAvailabilitySchedule==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }

        var response = dbAvailabilitySchedule.Select(schedule => new AvailabilityScheduleResponseModel
        {
            Id = schedule.Id,
            EmployeeId = schedule.EmployeeId,
            Description = schedule.Description,
            DayOfWeek = schedule.DayOfWeek,
            AvailableSlots = schedule.AvailableSlots,
        });

        return response;
    }

    public AvailabilityScheduleResponseModel GetById(int id)
    {
        var dbAvailabilitySchedule = _repository.FindById(id);
        if (dbAvailabilitySchedule==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }
        

        var response = new AvailabilityScheduleResponseModel()
        {
            Id = dbAvailabilitySchedule.Id,
            EmployeeId = dbAvailabilitySchedule.EmployeeId,
            Description = dbAvailabilitySchedule.Description,
            DayOfWeek = dbAvailabilitySchedule.DayOfWeek,
            AvailableSlots = dbAvailabilitySchedule.AvailableSlots,
        };

        return response;
    }

    public AvailabilityScheduleResponseModel Add(AvailabilityScheduleRequestModel request)
    {
        var requestToCreate = request as CreateAvailabilityScheduleRequest;
        if (requestToCreate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(AvailabilityScheduleEntity));
        }

        var availabilitySchedule = new AvailabilityScheduleEntity()
        {
            EmployeeId = requestToCreate.EmployeeId,
            Description = requestToCreate.Description,
            DayOfWeek = requestToCreate.DayOfWeek,
            AvailableSlots = requestToCreate.AvailableSlots
        };
        
        _repository.Add(availabilitySchedule);
        _repository.SaveChanges();

        var response = new AvailabilityScheduleResponseModel()
        {
            Id = availabilitySchedule.Id,
            EmployeeId = availabilitySchedule.EmployeeId,
            Description = availabilitySchedule.Description,
            DayOfWeek = availabilitySchedule.DayOfWeek,
            AvailableSlots = availabilitySchedule.AvailableSlots,
        };

        return response;
    }

    public AvailabilityScheduleResponseModel Update(int id, AvailabilityScheduleRequestModel request)
    {
        var dbAvailabilitySchedule = _repository.FindById(id);
        if (dbAvailabilitySchedule==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }

        var requestToUpdate = request as UpdateAvailabilityScheduleRequest;
        if (requestToUpdate==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(AvailabilityScheduleEntity));
        }

        dbAvailabilitySchedule.EmployeeId = requestToUpdate.EmployeeId;
        dbAvailabilitySchedule.Description = requestToUpdate.Description;
        dbAvailabilitySchedule.DayOfWeek = requestToUpdate.DayOfWeek;
        dbAvailabilitySchedule.AvailableSlots = requestToUpdate.AvailableSlots;
        

        _repository.Update(dbAvailabilitySchedule);
        _repository.SaveChanges();
        
        var response = new AvailabilityScheduleResponseModel()
        {
            Id = dbAvailabilitySchedule.Id,
            EmployeeId = dbAvailabilitySchedule.EmployeeId,
            Description = dbAvailabilitySchedule.Description,
            DayOfWeek = dbAvailabilitySchedule.DayOfWeek,
            AvailableSlots = dbAvailabilitySchedule.AvailableSlots,
            
        };

        return response;
    }

    public bool Delete(int id)
    {
        var dbAvailabilitySchedule = _repository.FindById(id);
        if (dbAvailabilitySchedule==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }
        
        _repository.Delete(dbAvailabilitySchedule);
        _repository.SaveChanges();

        return true;
    }
}