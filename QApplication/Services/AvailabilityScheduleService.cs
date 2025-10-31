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
            GroupId = schedule.GroupId,
            Description = schedule.Description,
            RepeatSlot = schedule.RepeatSlot,
            RepeatDuration = schedule.RepeatDuration,
            AvailableSlots = schedule.AvailableSlots,
            DayOfWeek = schedule.AvailableSlots.First().From.DayOfWeek
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
            GroupId = dbAvailabilitySchedule.GroupId,
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

        var crossDay = new List<Interval<DateTimeOffset>>();
        foreach (var slot in requestToCreate.AvailableSlots)
        {
            if (slot.From == slot.To)
            {
                throw new Exception("'From' and 'To' cannot be the same time.");
            }

            DateTimeOffset newTo;
            if (slot.To < slot.From)
            {
                newTo = slot.To.AddDays(1);
            }
            else
            {
                newTo = slot.To;
            }

            crossDay.Add(new Interval<DateTimeOffset>(slot.From, newTo));
        }

        requestToCreate.AvailableSlots = crossDay;

        if (requestToCreate.RepeatSlot != RepeatSlot.None)
        {
            if (!requestToCreate.RepeatDuration.HasValue || requestToCreate.RepeatDuration.Value <= 0)
            {
                throw new Exception("Repeat duration must be greater than 0");
            }
        }

        if (requestToCreate.RepeatSlot == RepeatSlot.None)
        {
            requestToCreate.RepeatDuration = 0;
        }


        int? nextGroupId = (_repository.GetAllSchedules().Max(s => s.GroupId) ?? 0) + 1;
        if (requestToCreate.RepeatSlot == RepeatSlot.None)
        {
            nextGroupId = null;
        }

        var scheduleInterval = new List<Interval<DateTimeOffset>>();

        for (int i = 0; i <= requestToCreate.RepeatDuration.Value; i++)
        {
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
                    case RepeatSlot.None:
                        break;
                }

                scheduleInterval.Add(new Interval<DateTimeOffset>(from, to));
            }
        }


        var schedulesByEmployee = _repository.GetEmployeeById(requestToCreate.EmployeeId).ToList();
        foreach (var schedule in schedulesByEmployee)
        {
            foreach (var slot in schedule.AvailableSlots)
            {
                var slotFrom = slot.From;
                DateTimeOffset slotTo;
                if (slot.To< slot.From)
                {
                    slotTo = slot.To.AddDays(1);
                }
                else
                {
                    slotTo = slot.To;
                }
                foreach (var newSlot in scheduleInterval)
                {
                    var newFrom = newSlot.From;
                    DateTimeOffset newTo;
                    if (newSlot.To< newSlot.From)
                    {
                        newTo = newSlot.To.AddDays(1);
                    }
                    else
                    {
                        newTo = newSlot.To;
                    }
                    
                    bool overlap = !(newTo <= slotFrom || newFrom >= slotTo);
                    if (overlap)
                    {
                        throw new Exception("This time slot already exists or overlaps with an existing schedule.");
                    }
                }
            }
        }

        var schedules = new List<AvailabilityScheduleEntity>();
        foreach (var interval in scheduleInterval)
        {
            schedules.Add(new AvailabilityScheduleEntity
            {
                EmployeeId = requestToCreate.EmployeeId,
                GroupId = nextGroupId,
                Description = requestToCreate.Description,
                AvailableSlots = new List<Interval<DateTimeOffset>> { interval },
                RepeatSlot = requestToCreate.RepeatSlot,
                RepeatDuration = requestToCreate.RepeatDuration
            });
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
            GroupId = slot.GroupId,
            Description = slot.Description,
            RepeatSlot = slot.RepeatSlot,
            RepeatDuration = slot.RepeatDuration,
            AvailableSlots = slot.AvailableSlots,
            DayOfWeek = slot.AvailableSlots.First().From.DayOfWeek
        }).ToList();

        return responses;
    }

    public AvailabilityScheduleResponseModel Update(int id, UpdateAvailabilityScheduleRequest requestToUpdate)
    {
        var dbAvailabilitySchedule = _repository.FindById(id);
        if (dbAvailabilitySchedule == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }

        var employee = _employeeRepository.FindById(dbAvailabilitySchedule.EmployeeId);
        if (employee == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        if (requestToUpdate.AvailableSlots == null || !requestToUpdate.AvailableSlots.Any())
        {
            throw new Exception("At least one available time slot must be provided");
        }

        var crossDay = new List<Interval<DateTimeOffset>>();
        foreach (var slot in requestToUpdate.AvailableSlots)
        {
            if (slot.From == slot.To)
            {
                throw new Exception("'From' must be earlier than 'To'");
            }

            DateTimeOffset newTo;
            if (slot.To < slot.From)
            {
                newTo = slot.To.AddDays(1);
            }
            else
            {
                newTo = slot.To;
            }

            crossDay.Add(new Interval<DateTimeOffset>(slot.From, newTo));
        }

        requestToUpdate.AvailableSlots = crossDay;

        if (requestToUpdate.RepeatSlot != RepeatSlot.None)
        {
            if (!requestToUpdate.RepeatDuration.HasValue || requestToUpdate.RepeatDuration.Value <= 0)
            {
                throw new Exception("Repeat duration must be greater than 0");
            }
        }

        List<AvailabilityScheduleEntity> schedulesToUpdate = new List<AvailabilityScheduleEntity>();

        if (dbAvailabilitySchedule.GroupId.HasValue && requestToUpdate.UpdateAllSlots)
        {
            schedulesToUpdate = _repository.GetAllSchedules()
                .Where(s => s.GroupId == dbAvailabilitySchedule.GroupId)
                .ToList();

            if (!schedulesToUpdate.Any())
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "No schedule found for the given GroupId");
            }
        }
        else
        {
            schedulesToUpdate.Add(dbAvailabilitySchedule);
        }


        var schedulesByEmployee = _repository.GetEmployeeById(employee.Id)
            .Where(s => !schedulesToUpdate.Contains(s))
            .ToList();

        foreach (var schedule in schedulesByEmployee)
        {
            foreach (var slot in schedule.AvailableSlots)
            {
                var slotFrom = slot.From;
                DateTimeOffset slotTo;
                if (slot.To < slot.From)
                {
                    slotTo = slot.To.AddDays(1);
                }
                else
                {
                    slotTo = slot.To;
                }

                foreach (var newSlot in requestToUpdate.AvailableSlots)
                {
                    var newFrom = newSlot.From;
                    DateTimeOffset newTo;
                    if (newSlot.To < slot.From)
                    {
                        newTo = newSlot.To.AddDays(1);
                    }
                    else
                    {
                        newTo = newSlot.To;
                    }

                    bool overlap = !(newTo <= slotFrom || newFrom >= slotTo);
                    if (overlap)
                    {
                        throw new Exception("This time slot already exists or overlaps with an existing schedule.");
                    }
                }
            }
        }

        foreach (var schedule in schedulesToUpdate)
        {
            schedule.Description = requestToUpdate.Description;
            schedule.AvailableSlots = requestToUpdate.AvailableSlots;
        }


        if ((requestToUpdate.RepeatSlot != dbAvailabilitySchedule.RepeatSlot) ||
            (requestToUpdate.RepeatDuration != dbAvailabilitySchedule.RepeatDuration))
        {
            if (requestToUpdate.RepeatDuration < dbAvailabilitySchedule.RepeatDuration)
            {
                var index = dbAvailabilitySchedule.RepeatDuration.Value - requestToUpdate.RepeatDuration.Value;
                for (int i = schedulesToUpdate.Count - index; i < schedulesToUpdate.Count; i++)
                {
                    _repository.Delete(schedulesToUpdate[i]);
                }
            }

            if (requestToUpdate.RepeatDuration > dbAvailabilitySchedule.RepeatDuration)
            {
                var index = requestToUpdate.RepeatDuration.Value - dbAvailabilitySchedule.RepeatDuration.Value;
                var baseSchedule = schedulesToUpdate.First();
                for (int i = 1; i < index; i++)
                {
                    var newSlots = new List<Interval<DateTimeOffset>>();
                    foreach (var slot in baseSchedule.AvailableSlots)
                    {
                        var from = slot.From;
                        var to = slot.To;
                        switch (requestToUpdate.RepeatSlot)
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
                            case RepeatSlot.None:
                                break;
                        }

                        newSlots.Add(new Interval<DateTimeOffset>(from, to));
                    }

                    _repository.Add(new AvailabilityScheduleEntity
                    {
                        EmployeeId = baseSchedule.Id,
                        GroupId = baseSchedule.GroupId,
                        Description = requestToUpdate.Description,
                        AvailableSlots = newSlots,
                        RepeatSlot = requestToUpdate.RepeatSlot,
                        RepeatDuration = requestToUpdate.RepeatDuration
                    });
                }
            }

            if (requestToUpdate.RepeatSlot != dbAvailabilitySchedule.RepeatSlot)
            {
                var baseSchedule = schedulesToUpdate.First();
                for (int i = 1; i < schedulesToUpdate.Count; i++)
                {
                    _repository.Delete(schedulesToUpdate[i]);
                }

                for (int i = 1; i < requestToUpdate.RepeatDuration.Value; i++)
                {
                    var newSlots = new List<Interval<DateTimeOffset>>();
                    foreach (var slot in baseSchedule.AvailableSlots)
                    {
                        var from = slot.From;
                        var to = slot.To;
                        switch (requestToUpdate.RepeatSlot)
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
                            case RepeatSlot.None:
                                break;
                        }

                        newSlots.Add(new Interval<DateTimeOffset>(from, to));
                    }

                    _repository.Add(new AvailabilityScheduleEntity
                    {
                        EmployeeId = baseSchedule.EmployeeId,
                        GroupId = baseSchedule.GroupId,
                        Description = requestToUpdate.Description,
                        AvailableSlots = newSlots,
                        RepeatSlot = requestToUpdate.RepeatSlot,
                        RepeatDuration = requestToUpdate.RepeatDuration
                    });
                }
            }

            dbAvailabilitySchedule.RepeatSlot = requestToUpdate.RepeatSlot;
            dbAvailabilitySchedule.RepeatDuration = requestToUpdate.RepeatDuration;
        }


        _repository.Update(dbAvailabilitySchedule);
        _repository.SaveChanges();

        var response = new AvailabilityScheduleResponseModel()
        {
            Id = dbAvailabilitySchedule.Id,
            EmployeeId = dbAvailabilitySchedule.EmployeeId,
            GroupId = dbAvailabilitySchedule.GroupId,
            Description = dbAvailabilitySchedule.Description,
            AvailableSlots = dbAvailabilitySchedule.AvailableSlots,
            RepeatSlot = dbAvailabilitySchedule.RepeatSlot,
            RepeatDuration = dbAvailabilitySchedule.RepeatDuration,
            DayOfWeek = dbAvailabilitySchedule.AvailableSlots.First().From.DayOfWeek
        };

        return response;
    }

    public bool Delete(int id, bool deleteAllSlots)
    {
        var dbAvailabilitySchedule = _repository.FindById(id);
        if (dbAvailabilitySchedule == null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }


        if (deleteAllSlots)
        {
            if (dbAvailabilitySchedule.GroupId == null)
            {
                throw new Exception("This schedule is not part of a group, nothing to delete in group.");
            }

            var scheduleToDelete = _repository.GetAllSchedules()
                .Where(s => s.GroupId == dbAvailabilitySchedule.GroupId).ToList();

            if (!scheduleToDelete.Any())
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "No schedule found for the given GroupId");
            }

            foreach (var schedule in scheduleToDelete)
            {
                _repository.Delete(schedule);
            }
        }
        else
        {
            _repository.Delete(dbAvailabilitySchedule);
        }

        _repository.SaveChanges();

        return true;
    }
}