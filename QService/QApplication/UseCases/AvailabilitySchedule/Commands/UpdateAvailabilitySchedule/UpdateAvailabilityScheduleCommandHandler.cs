using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.AvailabilitySchedule.Commands.UpdateAvailabilitySchedule;

public class UpdateAvailabilityScheduleCommandHandler: IRequestHandler<UpdateAvailabilityScheduleCommand, AvailabilityScheduleResponseModel>
{
    private readonly ILogger<UpdateAvailabilityScheduleCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public UpdateAvailabilityScheduleCommandHandler(ILogger<UpdateAvailabilityScheduleCommandHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<AvailabilityScheduleResponseModel> Handle(UpdateAvailabilityScheduleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating schedule with Id: {id}", request.Id);

        
        var dbAvailabilitySchedule =
            await _dbContext.AvailabilitySchedules.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (dbAvailabilitySchedule == null)
        {
            _logger.LogWarning("Schedule with Id {ScheduleId} not found for update", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(AvailabilityScheduleEntity));
        }

       
        var employee =
            await _dbContext.Employees.FirstOrDefaultAsync(s => s.Id == dbAvailabilitySchedule.EmployeeId,
                cancellationToken);

        if (employee == null)
        {
            _logger.LogWarning("Employee with Id {EmployeeId} not found for schedule update",
                dbAvailabilitySchedule.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        if (request.AvailableSlots == null || !request.AvailableSlots.Any())
        {
            _logger.LogError("Invalid available slot. At least one available time slot must be provided");
            throw new Exception("At least one available time slot must be provided");
        }

        _logger.LogDebug("Cross day checking");
        var crossDay = new List<Interval<DateTimeOffset>>();
        foreach (var slot in request.AvailableSlots)
        {
            if (slot.From == slot.To)
            {
                _logger.LogError("Invalid slot time. 'From' and 'To' cannot be the same time.");
                throw new Exception("'From' must be earlier than 'To'");
            }

            DateTimeOffset newTo = slot.To < slot.From ? slot.To.AddDays(1) : slot.To;
            crossDay.Add(new Interval<DateTimeOffset>(slot.From, newTo));
        }

        request.AvailableSlots = crossDay;

        if (request.RepeatSlot != RepeatSlot.None)
        {
            if (!request.RepeatDuration.HasValue || request.RepeatDuration.Value <= 0)
            {
                _logger.LogError("Invalid repeat duration. Repeat duration must be greater than 0");
                throw new Exception("Repeat duration must be greater than 0");
            }

            _logger.LogDebug("Repeat Slot: {slot}, Duration: {duration}", request.RepeatSlot,
                request.RepeatDuration);
        }

        List<AvailabilityScheduleEntity> schedulesToUpdate = new List<AvailabilityScheduleEntity>();
        var allSchedules = _dbContext.AvailabilitySchedules;
        if (dbAvailabilitySchedule.GroupId.HasValue && request.UpdateAllSlots)
        {
            _logger.LogDebug("Updating all slots in group {id}", dbAvailabilitySchedule.GroupId);
            schedulesToUpdate = await allSchedules
                .Where(s => s.GroupId == dbAvailabilitySchedule.GroupId)
                .ToListAsync(cancellationToken);

            if (!schedulesToUpdate.Any())
            {
                _logger.LogWarning("No schedules found for GroupId {id}", dbAvailabilitySchedule.GroupId);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "No schedule found for the given GroupId");
            }
        }
        else
        {
            schedulesToUpdate.Add(dbAvailabilitySchedule);
            _logger.LogDebug("Updating single schedule with Id: {id}", request.Id);
        }

        List<AvailabilityScheduleEntity> schedulesByEmployee;
        var employeeById = _dbContext.AvailabilitySchedules.Where(s => s.EmployeeId == employee.Id);
        if (dbAvailabilitySchedule.GroupId.HasValue)
        {
            if (request.UpdateAllSlots)
            {
                schedulesByEmployee = await employeeById
                    .Where(s => !schedulesToUpdate.Contains(s)).ToListAsync(cancellationToken);
                _logger.LogDebug("Checking overlap against schedules outside GroupId {groupId}",
                    dbAvailabilitySchedule.GroupId);
            }
            else
            {
                schedulesByEmployee = await allSchedules
                    .Where(s => s.EmployeeId == employee.Id &&
                                (!s.GroupId.HasValue || s.GroupId != dbAvailabilitySchedule.GroupId)).ToListAsync(cancellationToken);
                _logger.LogDebug("Checking overlap only outside group because UpdateAllSlots = false");
            }
        }
        else
        {
            schedulesByEmployee = await allSchedules
                .Where(s => s.EmployeeId == employee.Id && s.Id != dbAvailabilitySchedule.Id).ToListAsync(cancellationToken);
        }


        _logger.LogDebug("Checking for schedule overlap.");
        foreach (var schedule in schedulesByEmployee)
        {
            foreach (var slot in schedule.AvailableSlots)
            {
                var slotFrom = slot.From;
                DateTimeOffset slotTo = slot.To < slot.From ? slot.To.AddDays(1) : slot.To;

                foreach (var newSlot in request.AvailableSlots)
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
            schedule.Description = request.Description;
            schedule.AvailableSlots = request.AvailableSlots;
        }


        bool repeatChanged = request.RepeatSlot != dbAvailabilitySchedule.RepeatSlot ||
                             request.RepeatDuration != dbAvailabilitySchedule.RepeatDuration;

        if (repeatChanged && request.UpdateAllSlots)
        {
            _logger.LogInformation("Repeat slot or repeat duration changed.");

            if (!dbAvailabilitySchedule.GroupId.HasValue)
            {
                dbAvailabilitySchedule.GroupId = (allSchedules.Max(s => s.GroupId) ?? 0) + 1;
                _logger.LogDebug("Assigned new GroupId: {groupId}", dbAvailabilitySchedule.GroupId);
            }


            var oldGroupSchedules = await allSchedules
                .Where(s => s.GroupId == dbAvailabilitySchedule.GroupId && s.Id != dbAvailabilitySchedule.Id)
                .ToListAsync(cancellationToken);

            foreach (var oldSchedule in oldGroupSchedules)
            {
                _dbContext.AvailabilitySchedules.Remove(oldSchedule);
                _logger.LogDebug("Deleted old schedule Id: {id}", oldSchedule.Id);
            }


            if (request.RepeatSlot != RepeatSlot.None)
            {
                int totalSchedules = request.RepeatDuration.Value;
                var baseSchedule = dbAvailabilitySchedule;

                for (int i = 1; i < totalSchedules; i++)
                {
                    var newSlot = new List<Interval<DateTimeOffset>>();
                    foreach (var slot in baseSchedule.AvailableSlots)
                    {
                        var from = slot.From;
                        var to = slot.To;
                        switch (request.RepeatSlot)
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

                    await _dbContext.AvailabilitySchedules.AddAsync(new AvailabilityScheduleEntity
                    {
                        EmployeeId = baseSchedule.EmployeeId,
                        GroupId = baseSchedule.GroupId,
                        Description = request.Description,
                        AvailableSlots = newSlot,
                        RepeatSlot = request.RepeatSlot,
                        RepeatDuration = request.RepeatDuration,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken);

                    _logger.LogDebug("Added new repeated schedule for EmployeeId: {employeeId}",
                        baseSchedule.EmployeeId);
                }
            }
            else
            {
                dbAvailabilitySchedule.GroupId = null;
                dbAvailabilitySchedule.RepeatDuration = 1;
            }

            dbAvailabilitySchedule.RepeatSlot = request.RepeatSlot;
            dbAvailabilitySchedule.RepeatDuration = request.RepeatDuration;
        }

        _logger.LogDebug("Saving updated schedules to repository");
       
        await _dbContext.SaveChangesAsync(cancellationToken);

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

        _logger.LogInformation("Successfully updated schedule with Id {id}", request.Id);
        return response;
    }
}