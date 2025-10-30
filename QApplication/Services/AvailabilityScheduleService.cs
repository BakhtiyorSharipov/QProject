using System.Net;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.AvailabilityScheduleRequest;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Services;

public class AvailabilityScheduleService : IAvailabilityScheduleService
{
    private readonly IAvailabilityScheduleRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;

    public AvailabilityScheduleService(IAvailabilityScheduleRepository repository,
        IEmployeeRepository employeeRepository)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
    }

    public IEnumerable<AvailabilityScheduleResponseModel> GetAll(int pageList, int pageNumber)
    {
        var dbAvailabilitySchedule = _repository.GetAll(pageList, pageNumber);
        if (dbAvailabilitySchedule == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }

        var response = dbAvailabilitySchedule.Select(schedule => new AvailabilityScheduleResponseModel
        {
            Id = schedule.Id,
            EmployeeId = schedule.EmployeeId,
            Description = schedule.Description,
            AvailableSlots = schedule.AvailableSlots,
        });

        return response;
    }

    public AvailabilityScheduleResponseModel GetById(int id)
    {
        var dbAvailabilitySchedule = _repository.FindById(id);
        if (dbAvailabilitySchedule == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }


        var response = new AvailabilityScheduleResponseModel()
        {
            Id = dbAvailabilitySchedule.Id,
            EmployeeId = dbAvailabilitySchedule.EmployeeId,
            Description = dbAvailabilitySchedule.Description,
            RepeatSlot = dbAvailabilitySchedule.RepeatSlot,
            RepeatDuration = dbAvailabilitySchedule.RepeatDuration,
            AvailableSlots = dbAvailabilitySchedule.AvailableSlots,
            DayOfWeek = dbAvailabilitySchedule.AvailableSlots.First().From.DayOfWeek
        };

        return response;
    }

    public IEnumerable<AvailabilityScheduleResponseModel> Add(AvailabilityScheduleRequestModel request)
    {
        var requestToCreate = request as CreateAvailabilityScheduleRequest;
        if (requestToCreate == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(AvailabilityScheduleEntity));
        }

        var employee = _employeeRepository.FindById(request.EmployeeId);
        if (employee == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        if (requestToCreate.AvailableSlots == null || !requestToCreate.AvailableSlots.Any())
        {
            throw new Exception("At least one available time slot must be provided");
        }

        foreach (var slot in requestToCreate.AvailableSlots)
        {
            if (slot.From >= slot.To)
            {
                throw new Exception("'From' must be earlier than 'To'");
            }
        }

        
        
        var schedulesByEmployee = _repository.GetEmployeeById(requestToCreate.EmployeeId).ToList();

        foreach (var schedule in schedulesByEmployee)
        {
            var newSlotTo = DateTimeOffset.Now;
            var newSlotFrom = DateTimeOffset.Now;
            foreach (var requestSlot in requestToCreate.AvailableSlots)
            {
                newSlotFrom = requestSlot.From;
                newSlotTo = requestSlot.To;
            }

            foreach (var slot in schedule.AvailableSlots)
            {
                bool overlap = !(newSlotTo <= slot.From || newSlotFrom >= slot.To);
                if (overlap)
                {
                    throw new Exception("This time slot already exists or overlaps with an existing schedule.");
                }
            }
        }
        
        if (requestToCreate.RepeatSlot != RepeatSlot.None)
        {
            if (!requestToCreate.RepeatDuration.HasValue || requestToCreate.RepeatDuration.Value <= 0)
            {
                throw new Exception("Repeat duration must be greater than 0");
            }
        }

        var schedules = new List<AvailabilityScheduleEntity>();
        if (requestToCreate.RepeatSlot == RepeatSlot.None)
        {
            requestToCreate.RepeatDuration = 0;
            schedules.Add(new AvailabilityScheduleEntity
            {
                EmployeeId = requestToCreate.EmployeeId,
                Description = requestToCreate.Description,
                AvailableSlots = requestToCreate.AvailableSlots,
                RepeatSlot = requestToCreate.RepeatSlot,
                RepeatDuration = requestToCreate.RepeatDuration
            });
        }
        else
        {
            for (int i = 0; i < requestToCreate.RepeatDuration.Value; i++)
            {
                var scheduleInterval = new List<Interval<DateTimeOffset>>();
                foreach (var slot in requestToCreate.AvailableSlots)
                {
                    var from = slot.From;
                    var to = slot.To;
                    switch (requestToCreate.RepeatSlot)
                    {
                        case RepeatSlot.Daily:
                            from = from.AddDays(i);
                            to = to.AddDays(i);
                            break;
                        case RepeatSlot.Weekly:
                            from = from.AddDays(i * 7);
                            to = to.AddDays(i * 7);
                            break;
                        case RepeatSlot.BiWeekly:
                            from = from.AddDays(i * 14);
                            to = to.AddDays(i * 14);
                            break;
                        case RepeatSlot.TriWeekly:
                            from = from.AddDays(i * 21);
                            to = to.AddDays(i * 21);
                            break;
                        case RepeatSlot.TwiceAMonth:
                            from = from.AddDays(i * 15);
                            to = to.AddDays(i * 15);
                            break;
                        case RepeatSlot.ThreeTimesAMonth:
                            from = from.AddDays(i * 10);
                            to = to.AddDays(i * 10);
                            break;
                        case RepeatSlot.Monthly:
                            from = from.AddMonths(i);
                            to = to.AddMonths(i);
                            break;
                    }

                    scheduleInterval.Add(new Interval<DateTimeOffset>(from, to));
                }

                schedules.Add(new AvailabilityScheduleEntity
                {
                    EmployeeId = requestToCreate.EmployeeId,
                    Description = requestToCreate.Description,
                    AvailableSlots = scheduleInterval,
                    RepeatSlot = requestToCreate.RepeatSlot,
                    RepeatDuration = requestToCreate.RepeatDuration
                });
            }
        }


        foreach (var schedule in schedules)
        {
            _repository.Add(schedule);
        }

        _repository.SaveChanges();

        var responses = schedules.Select(slot => new AvailabilityScheduleResponseModel
        {
            Id = slot.Id,
            EmployeeId = slot.EmployeeId,
            Description = slot.Description,
            RepeatSlot = slot.RepeatSlot,
            RepeatDuration = slot.RepeatDuration,
            AvailableSlots = slot.AvailableSlots,
            DayOfWeek = slot.AvailableSlots.First().From.DayOfWeek
        }).ToList();

        return responses;
    }

    public AvailabilityScheduleResponseModel Update(int id, AvailabilityScheduleRequestModel request)
    {
        var dbAvailabilitySchedule = _repository.FindById(id);
        if (dbAvailabilitySchedule == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }

        var requestToUpdate = request as UpdateAvailabilityScheduleRequest;
        if (requestToUpdate == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(AvailabilityScheduleEntity));
        }

        if (requestToUpdate.AvailableSlots == null || !requestToUpdate.AvailableSlots.Any())
        {
            throw new Exception("At least one available time slot must be provided");
        }

        foreach (var slot in requestToUpdate.AvailableSlots)
        {
            if (slot.From >= slot.To)
            {
                throw new Exception("'From' must be earlier than 'To'");
            }
        }

        var sortedSlot = requestToUpdate.AvailableSlots.OrderBy(s => s.From).ToList();
        for (int i = 1; i < sortedSlot.Count; i++)
        {
            if (sortedSlot[i].From < sortedSlot[i - 1].To)
            {
                throw new Exception("Time slots can not overlap");
            }
        }

        if (requestToUpdate.RepeatSlot != RepeatSlot.None)
        {
            if (!requestToUpdate.RepeatDuration.HasValue || requestToUpdate.RepeatDuration.Value <= 0)
            {
                throw new Exception("Repeat duration must be greater than 0");
            }
        }

        dbAvailabilitySchedule.EmployeeId = requestToUpdate.EmployeeId;
        dbAvailabilitySchedule.Description = requestToUpdate.Description;
        dbAvailabilitySchedule.AvailableSlots = requestToUpdate.AvailableSlots;


        _repository.Update(dbAvailabilitySchedule);
        _repository.SaveChanges();

        var response = new AvailabilityScheduleResponseModel()
        {
            Id = dbAvailabilitySchedule.Id,
            EmployeeId = dbAvailabilitySchedule.EmployeeId,
            Description = dbAvailabilitySchedule.Description,
            AvailableSlots = dbAvailabilitySchedule.AvailableSlots,
        };

        return response;
    }

    public bool Delete(int id)
    {
        var dbAvailabilitySchedule = _repository.FindById(id);
        if (dbAvailabilitySchedule == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }

        _repository.Delete(dbAvailabilitySchedule);
        _repository.SaveChanges();

        return true;
    }
}