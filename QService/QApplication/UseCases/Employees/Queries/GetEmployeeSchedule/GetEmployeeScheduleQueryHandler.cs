using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses.AvailabilityResponse;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.Employees.Queries.GetEmployeeSchedule;

public class
    GetEmployeeScheduleQueryHandler : IRequestHandler<GetEmployeeScheduleQuery, GetEmployeeAvailabilityResponse>
{
    private readonly ILogger<GetEmployeeScheduleQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetEmployeeScheduleQueryHandler(ILogger<GetEmployeeScheduleQueryHandler> logger,
        IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<GetEmployeeAvailabilityResponse> Handle(GetEmployeeScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var employeeExist = await _dbContext.Employees
            .AnyAsync(s => s.Id == request.EmployeeId, cancellationToken);

        if (!employeeExist)
        {
            _logger.LogWarning("Employee with Id {employeeId} not found", request.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"Employee with Id {request.EmployeeId} not found");
        }


        var date = request.Date.Date;
        var nextDay = date.AddDays(1);

        var employeeSchedules = await _dbContext.AvailabilitySchedules
            .Where(s => s.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);

        if (!employeeSchedules.Any())
        {
            _logger.LogWarning("Not found any schedule for this employee");
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Not found any schedule for this employee");
        }

        var employeeQueues = await _dbContext.Queues
            .Where(s => s.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);

        var dayQueues = new List<TimeIntervalResponse>();

        foreach (var queue in employeeQueues)
        {
            var qEnd = queue.EndTime ?? queue.StartTime.AddMinutes(30);
            if (queue.StartTime < nextDay && qEnd > date)
            {
                dayQueues.Add(new TimeIntervalResponse
                {
                    Start = queue.StartTime,
                    End = qEnd
                });
            }
        }


        var timeline = new List<TimelineBlockResponse>();

        foreach (var schedule in employeeSchedules)
        {
            foreach (var slot in schedule.AvailableSlots)
            {
                if (slot.From >= nextDay || slot.To <= date)
                {
                    continue;
                }

                var scheduleStart = slot.From < date ? date : slot.From;
                var scheduleEnd = slot.To > nextDay ? nextDay : slot.To;

                var current = scheduleStart;

                foreach (var queue in dayQueues)
                {
                    if (queue.End <= scheduleStart || queue.Start >= scheduleEnd)
                    {
                        continue;
                    }

                    if (current < queue.Start)
                    {
                        timeline.Add(new TimelineBlockResponse
                        {
                            Start = current,
                            End = queue.Start,
                            Type = SlotType.Available
                        });
                    }

                    var bookedStart = queue.Start < scheduleStart ? scheduleStart : queue.Start;
                    var bookedEnd = queue.End > scheduleEnd ? scheduleEnd : queue.End;

                    timeline.Add(new TimelineBlockResponse
                    {
                        Start = bookedStart,
                        End = bookedEnd,
                        Type = SlotType.Booked
                    });

                    current = bookedEnd;
                }

                if (current < scheduleEnd)
                {
                    timeline.Add(new TimelineBlockResponse()
                    {
                        Start = current,
                        End = scheduleEnd,
                        Type = SlotType.Available
                    });
                }
            }
        }

        timeline = timeline.OrderBy(t => t.Start).ToList();

        return new GetEmployeeAvailabilityResponse
        {
            Days = new List<AvailabilityDayResponse>
            {
                new AvailabilityDayResponse()
                {
                    Date = date,
                    Timeline = timeline
                }
            }
        };
    }
}