using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Requests.AvailabilityScheduleRequest;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.AvailabilitySchedule.Commands.CreateAvailabilitySchedule;

public class CreateAvailabilityScheduleCommandHandler: IRequestHandler<CreateAvailabilityScheduleCommand, List<AvailabilityScheduleResponseModel>>
{
    private readonly ILogger<CreateAvailabilityScheduleCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public CreateAvailabilityScheduleCommandHandler(ILogger<CreateAvailabilityScheduleCommandHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<AvailabilityScheduleResponseModel>> Handle(CreateAvailabilityScheduleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding new schedule for EmployeeId {id}", request.EmployeeId);
        
        var employee =
            await _dbContext.Employees.FirstOrDefaultAsync(s => s.Id == request.EmployeeId, cancellationToken);
        if (employee == null)
        {
            _logger.LogWarning("Employee with Id {id} not found", request.EmployeeId);
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

        if (request.RepeatSlot == RepeatSlot.None)
        {
            request.RepeatDuration = 0;
            _logger.LogDebug("No repeat - duration is 0");
        }



        var allSchedules =  _dbContext.AvailabilitySchedules;
        
        int? nextGroupId = (allSchedules.Max(s => s.GroupId) ?? 0) + 1;

        if (request.RepeatSlot == RepeatSlot.None)
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
        for (int i = 0; i <= request.RepeatDuration.Value; i++)
        {
            foreach (var slot in request.AvailableSlots)
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

                scheduleInterval.Add(new Interval<DateTimeOffset>(from, to));
                _logger.LogDebug("Created interval: {from} to {to}", from, to);
            }
        }


        _logger.LogDebug("Checking for schedule overlap for EmployeeId: {id}", request.EmployeeId);
        var schedulesByEmployee =
            await _dbContext.AvailabilitySchedules.Where(s => s.EmployeeId == request.EmployeeId).ToListAsync(cancellationToken);

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
                            request.EmployeeId);
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
                EmployeeId = request.EmployeeId,
                GroupId = nextGroupId,
                Description = request.Description,
                AvailableSlots = new List<Interval<DateTimeOffset>> { interval },
                RepeatSlot = request.RepeatSlot,
                RepeatDuration = request.RepeatDuration,
                CreatedAt = DateTime.UtcNow
            });
        }


        foreach (var schedule in schedules)
        {
            await _dbContext.AvailabilitySchedules.AddAsync(schedule, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
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
}