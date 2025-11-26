using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<AvailabilityScheduleService> _logger;

    public AvailabilityScheduleService(IAvailabilityScheduleRepository repository,
        IEmployeeRepository employeeRepository, ILogger<AvailabilityScheduleService> logger)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AvailabilityScheduleResponseModel>> GetAllAsync(int pageList, int pageNumber)
    {
        _logger.LogInformation("Getting all schedules: PageNumber: {pageNumber}, PageList: {pageList}", pageNumber,
            pageList);
        var dbAvailabilitySchedule = await _repository.GetAll(pageList, pageNumber).ToListAsync();

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

        _logger.LogInformation("Fetched {scheduleCount} schedules.", response.Count());
        return response;
    }

    public async Task<AvailabilityScheduleResponseModel> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting schedule with Id {id}", id);
        var dbAvailabilitySchedule = await _repository.FindByIdAsync(id);
        if (dbAvailabilitySchedule == null)
        {
            _logger.LogWarning("Schedule with Id {id} not found", id);
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

        _logger.LogInformation("Schedule with Id {id} fetched successfully", id);
        return response;
    }

    public async Task<IEnumerable<AvailabilityScheduleResponseModel>> AddAsync(AvailabilityScheduleRequestModel request)
    {
        _logger.LogInformation("Adding new schedule for EmployeeId {id}", request.EmployeeId);
        var requestToCreate = request as CreateAvailabilityScheduleRequest;
        if (requestToCreate == null)
        {
            _logger.LogError("Invalid request model while adding new schedule.");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(AvailabilityScheduleEntity));
        }

        var employee = await _employeeRepository.FindByIdAsync(request.EmployeeId);
        if (employee == null)
        {
            _logger.LogWarning("Employee with Id {id} not found", request.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        if (requestToCreate.AvailableSlots == null || !requestToCreate.AvailableSlots.Any())
        {
            _logger.LogError("Invalid available slot. At least one available time slot must be provided");
            throw new Exception("At least one available time slot must be provided");
        }

        _logger.LogDebug("Cross day checking");
        var crossDay = new List<Interval<DateTimeOffset>>();
        foreach (var slot in requestToCreate.AvailableSlots)
        {
            if (slot.From == slot.To)
            {
                _logger.LogError("Invalid slot time. 'From' and 'To' cannot be the same time.");
                throw new Exception("'From' and 'To' cannot be the same time.");
            }

            DateTimeOffset newTo;
            if (slot.To < slot.From)
            {
                newTo = slot.To.AddDays(1);
                _logger.LogDebug("Day is cross day");
            }
            else
            {
                newTo = slot.To;
                _logger.LogDebug("Day is not cross day");
            }

            crossDay.Add(new Interval<DateTimeOffset>(slot.From, newTo));
        }

        requestToCreate.AvailableSlots = crossDay;

        if (requestToCreate.RepeatSlot != RepeatSlot.None)
        {
            if (!requestToCreate.RepeatDuration.HasValue || requestToCreate.RepeatDuration.Value <= 0)
            {
                _logger.LogError("Invalid repeat duration. Repeat duration must be greater than 0");
                throw new Exception("Repeat duration must be greater than 0");
            }

            _logger.LogDebug("Repeat Slot: {slot}, Duration: {duration}", requestToCreate.RepeatSlot,
                requestToCreate.RepeatDuration);
        }

        if (requestToCreate.RepeatSlot == RepeatSlot.None)
        {
            requestToCreate.RepeatDuration = 0;
            _logger.LogDebug("No repeat - duration is 0");
        }


        int? nextGroupId = (_repository.GetAllSchedules().Max(s => s.GroupId) ?? 0) + 1;
        if (requestToCreate.RepeatSlot == RepeatSlot.None)
        {
            nextGroupId = null;
            _logger.LogDebug("No group Id assigned for non-repeating schedule");
        }
        else
        {
            _logger.LogDebug("Assigned GroupId: {groupId}", nextGroupId);
        }

        var scheduleInterval = new List<Interval<DateTimeOffset>>();
        _logger.LogDebug("Creating schedule intervals");
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
                _logger.LogDebug("Created interval: {from} to {to}", from, to);
            }
        }


        _logger.LogDebug("Checking for schedule overlap for EmployeeId: {id}", requestToCreate.EmployeeId);
        var schedulesByEmployee = await _repository.GetEmployeeById(requestToCreate.EmployeeId).ToListAsync();
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

                foreach (var newSlot in scheduleInterval)
                {
                    var newFrom = newSlot.From;
                    DateTimeOffset newTo;
                    if (newSlot.To < newSlot.From)
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
                        _logger.LogError("Overlapping schedule for EmployeeId: {employeeId}",
                            requestToCreate.EmployeeId);
                        throw new Exception("This time slot already exists or overlaps with an existing schedule.");
                    }
                }
            }
        }


        _logger.LogDebug("Creating schedule entities.");
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
                RepeatDuration = requestToCreate.RepeatDuration,
                CreatedAt = DateTime.UtcNow
            });
        }


        foreach (var schedule in schedules)
        {
            await _repository.AddAsync(schedule);
        }

        await _repository.SaveChangesAsync();
        _logger.LogDebug("Schedules saved to repository.");

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

        _logger.LogInformation("Successfully added schedules for EmployeeId: {id}", request.EmployeeId);
        return responses;
    }

    public async Task<AvailabilityScheduleResponseModel> UpdateAsync(int id,
        UpdateAvailabilityScheduleRequest requestToUpdate, bool updateAllSlots)
    {
        _logger.LogInformation("Updating schedule with Id: {id}", id);

        var dbAvailabilitySchedule = await _repository.FindByIdAsync(id);
        if (dbAvailabilitySchedule == null)
        {
            _logger.LogWarning("Schedule with Id {ScheduleId} not found for update", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }

        var employee = await _employeeRepository.FindByIdAsync(dbAvailabilitySchedule.EmployeeId);
        if (employee == null)
        {
            _logger.LogWarning("Employee with Id {EmployeeId} not found for schedule update",
                dbAvailabilitySchedule.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        if (requestToUpdate.AvailableSlots == null || !requestToUpdate.AvailableSlots.Any())
        {
            _logger.LogError("Invalid available slot. At least one available time slot must be provided");
            throw new Exception("At least one available time slot must be provided");
        }

        _logger.LogDebug("Cross day checking");
        var crossDay = new List<Interval<DateTimeOffset>>();
        foreach (var slot in requestToUpdate.AvailableSlots)
        {
            if (slot.From == slot.To)
            {
                _logger.LogError("Invalid slot time. 'From' and 'To' cannot be the same time.");
                throw new Exception("'From' must be earlier than 'To'");
            }

            DateTimeOffset newTo = slot.To < slot.From ? slot.To.AddDays(1) : slot.To;
            crossDay.Add(new Interval<DateTimeOffset>(slot.From, newTo));
        }

        requestToUpdate.AvailableSlots = crossDay;

        if (requestToUpdate.RepeatSlot != RepeatSlot.None)
        {
            if (!requestToUpdate.RepeatDuration.HasValue || requestToUpdate.RepeatDuration.Value <= 0)
            {
                _logger.LogError("Invalid repeat duration. Repeat duration must be greater than 0");
                throw new Exception("Repeat duration must be greater than 0");
            }

            _logger.LogDebug("Repeat Slot: {slot}, Duration: {duration}", requestToUpdate.RepeatSlot,
                requestToUpdate.RepeatDuration);
        }

        List<AvailabilityScheduleEntity> schedulesToUpdate = new List<AvailabilityScheduleEntity>();

        if (dbAvailabilitySchedule.GroupId.HasValue && updateAllSlots)
        {
            _logger.LogDebug("Updating all slots in group {id}", dbAvailabilitySchedule.GroupId);
            schedulesToUpdate = await _repository.GetAllSchedules()
                .Where(s => s.GroupId == dbAvailabilitySchedule.GroupId)
                .ToListAsync();

            if (!schedulesToUpdate.Any())
            {
                _logger.LogWarning("No schedules found for GroupId {id}", dbAvailabilitySchedule.GroupId);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "No schedule found for the given GroupId");
            }
        }
        else
        {
            schedulesToUpdate.Add(dbAvailabilitySchedule);
            _logger.LogDebug("Updating single schedule with Id: {id}", id);
        }

        List<AvailabilityScheduleEntity> schedulesByEmployee;
        if (dbAvailabilitySchedule.GroupId.HasValue)
        {
            if (updateAllSlots)
            {
                schedulesByEmployee = await _repository.GetEmployeeById(employee.Id)
                    .Where(s => !schedulesToUpdate.Contains(s)).ToListAsync();
                _logger.LogDebug("Checking overlap against schedules outside GroupId {groupId}",
                    dbAvailabilitySchedule.GroupId);
            }
            else
            {
                schedulesByEmployee = await _repository.GetAllSchedules()
                    .Where(s => s.EmployeeId == employee.Id &&
                                (!s.GroupId.HasValue || s.GroupId != dbAvailabilitySchedule.GroupId)).ToListAsync();
                _logger.LogDebug("Checking overlap only outside group because UpdateAllSlots = false");
            }
        }
        else
        {
            schedulesByEmployee = await _repository.GetAllSchedules()
                .Where(s => s.EmployeeId == employee.Id && s.Id != dbAvailabilitySchedule.Id).ToListAsync();
        }


        _logger.LogDebug("Checking for schedule overlap.");
        foreach (var schedule in schedulesByEmployee)
        {
            foreach (var slot in schedule.AvailableSlots)
            {
                var slotFrom = slot.From;
                DateTimeOffset slotTo = slot.To < slot.From ? slot.To.AddDays(1) : slot.To;

                foreach (var newSlot in requestToUpdate.AvailableSlots)
                {
                    var newFrom = newSlot.From;
                    DateTimeOffset newTo = newSlot.To < newSlot.From ? newSlot.To.AddDays(1) : newSlot.To;

                    bool overlap = !(newTo <= slotFrom || newFrom >= slotTo);
                    if (overlap)
                    {
                        _logger.LogError("Overlapping schedule for EmployeeId: {employeeId}", employee.Id);
                        throw new Exception("This time slot already exists or overlaps with an existing schedule.");
                    }
                }
            }
        }


        _logger.LogDebug("Updating schedule entities");
        foreach (var schedule in schedulesToUpdate)
        {
            schedule.Description = requestToUpdate.Description;
            schedule.AvailableSlots = requestToUpdate.AvailableSlots;
        }


        bool repeatChanged = requestToUpdate.RepeatSlot != dbAvailabilitySchedule.RepeatSlot ||
                             requestToUpdate.RepeatDuration != dbAvailabilitySchedule.RepeatDuration;

        if (repeatChanged && updateAllSlots)
        {
            _logger.LogInformation("Repeat slot or repeat duration changed.");

            if (!dbAvailabilitySchedule.GroupId.HasValue)
            {
                dbAvailabilitySchedule.GroupId = (_repository.GetAllSchedules().Max(s => s.GroupId) ?? 0) + 1;
                _logger.LogDebug("Assigned new GroupId: {groupId}", dbAvailabilitySchedule.GroupId);
            }


            var oldGroupSchedules = await _repository.GetAllSchedules()
                .Where(s => s.GroupId == dbAvailabilitySchedule.GroupId && s.Id != dbAvailabilitySchedule.Id)
                .ToListAsync();

            foreach (var oldSchedule in oldGroupSchedules)
            {
                _repository.Delete(oldSchedule);
                _logger.LogDebug("Deleted old schedule Id: {id}", oldSchedule.Id);
            }


            if (requestToUpdate.RepeatSlot != RepeatSlot.None)
            {
                int totalSchedules = requestToUpdate.RepeatDuration.Value;
                var baseSchedule = dbAvailabilitySchedule;

                for (int i = 1; i < totalSchedules; i++)
                {
                    var newSlot = new List<Interval<DateTimeOffset>>();
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

                        newSlot.Add(new Interval<DateTimeOffset>(from, to));
                    }

                    await _repository.AddAsync(new AvailabilityScheduleEntity
                    {
                        EmployeeId = baseSchedule.EmployeeId,
                        GroupId = baseSchedule.GroupId,
                        Description = requestToUpdate.Description,
                        AvailableSlots = newSlot,
                        RepeatSlot = requestToUpdate.RepeatSlot,
                        RepeatDuration = requestToUpdate.RepeatDuration,
                        CreatedAt = DateTime.UtcNow
                    });

                    _logger.LogDebug("Added new repeated schedule for EmployeeId: {employeeId}",
                        baseSchedule.EmployeeId);
                }
            }
            else
            {
                dbAvailabilitySchedule.GroupId = null;
                dbAvailabilitySchedule.RepeatDuration = 1;
            }

            dbAvailabilitySchedule.RepeatSlot = requestToUpdate.RepeatSlot;
            dbAvailabilitySchedule.RepeatDuration = requestToUpdate.RepeatDuration;
        }

        _logger.LogDebug("Saving updated schedules to repository");
        _repository.Update(dbAvailabilitySchedule);
        await _repository.SaveChangesAsync();

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

        _logger.LogInformation("Successfully updated schedule with Id {id}", id);
        return response;
    }

    public async Task<bool> DeleteAsync(int id, bool deleteAllSlots)
    {
        _logger.LogInformation("Deleting schedule with Id: {id}, DeleteAllSlots: {delete}", id, deleteAllSlots);
        var dbAvailabilitySchedule = await _repository.FindByIdAsync(id);
        if (dbAvailabilitySchedule == null)
        {
            _logger.LogWarning("Schedule with Id {id} not found for deleting", id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }


        if (deleteAllSlots)
        {
            if (dbAvailabilitySchedule.GroupId == null)
            {
                _logger.LogDebug("GroupId == null");
                throw new Exception("This schedule is not part of a group, nothing to delete in group.");
            }

            var scheduleToDelete = await _repository.GetAllSchedules()
                .Where(s => s.GroupId == dbAvailabilitySchedule.GroupId).ToListAsync();

            if (!scheduleToDelete.Any())
            {
                _logger.LogWarning("No schedules found for GroupId {id}", dbAvailabilitySchedule.GroupId);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "No schedule found for the given GroupId");
            }

            foreach (var schedule in scheduleToDelete)
            {
                _repository.Delete(schedule);
                _logger.LogDebug("Deleted schedule Id {id} from group Id {groupId}", schedule.Id, schedule.GroupId);
            }
        }
        else
        {
            _repository.Delete(dbAvailabilitySchedule);
            _logger.LogDebug("Deleted single schedule with Id: {schduleId}", id);
        }

        await _repository.SaveChangesAsync();
        _logger.LogInformation("Deletion successfully completed for schedule Id {id}", id);

        return true;
    }
}