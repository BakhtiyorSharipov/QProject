using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses.AvailabilityResponse;
using QDomain.Enums;

namespace QApplication.UseCases.Employees.Queries.GetEmployeeSchedule;

public class GetEmployeeScheduleQueryHandler : IRequestHandler<GetEmployeeScheduleQuery, GetEmployeeAvailabilityResponse>
{
    private readonly ILogger<GetEmployeeScheduleQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;

    public GetEmployeeScheduleQueryHandler(ILogger<GetEmployeeScheduleQueryHandler> logger,
        IQueueApplicationDbContext dbContext, ICacheService cache)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
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

        
        var baseSchedule = await _cache.GetBaseSchedule(request.EmployeeId, date);
        if (baseSchedule == null)
        {
            _logger.LogInformation("Base schedule cache MISS");
            var schedules = await _dbContext.AvailabilitySchedules
                .Where(s => s.EmployeeId == request.EmployeeId)
                .ToListAsync(cancellationToken);
            baseSchedule = new List<TimelineBlockResponse>();

            foreach (var schedule in schedules)
            {
                foreach (var slot in schedule.AvailableSlots)
                {
                    if (slot.From >= nextDay || slot.To <= date)
                        continue;

                    var start = slot.From < date ? date : slot.From;
                    var end = slot.To > nextDay ? nextDay : slot.To;

                    baseSchedule.Add(new TimelineBlockResponse
                    {
                        Start = start,
                        End = end,
                        Type = SlotType.Available
                    });
                }
            }

            await _cache.SetBaseSchedule(request.EmployeeId, date, baseSchedule);
        }
        var queues = await _cache.GetQueuesFromSchedule(request.EmployeeId, date);
        var timeline = new List<TimelineBlockResponse>();
        
        foreach (var slot in baseSchedule)
        {
            var current = slot.Start;

            foreach (var queue in queues.OrderBy(q => q.Start))
            {
                if (queue.End <= slot.Start || queue.Start >= slot.End)
                    continue;

                if (current < queue.Start)
                {
                    timeline.Add(new TimelineBlockResponse
                    {
                        Start = current,
                        End = queue.Start,
                        Type = SlotType.Available
                    });
                }

                timeline.Add(new TimelineBlockResponse
                {
                    Start = queue.Start,
                    End = queue.End,
                    Type = SlotType.Booked
                });

                current = queue.End;
            }

            if (current < slot.End)
            {
                timeline.Add(new TimelineBlockResponse
                {
                    Start = current,
                    End = slot.End,
                    Type = SlotType.Available
                });
            }
        }

        return new GetEmployeeAvailabilityResponse
        {
            Days = new List<AvailabilityDayResponse>
            {
                new()
                {
                    Date = date,
                    Timeline = timeline.OrderBy(t => t.Start).ToList()
                }
            }
        };
        
    }
}