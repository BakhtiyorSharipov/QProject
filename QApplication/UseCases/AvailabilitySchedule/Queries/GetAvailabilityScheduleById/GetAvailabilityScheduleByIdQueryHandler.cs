using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.AvailabilitySchedule.Queries.GetAvailabilityScheduleById;

public class GetAvailabilityScheduleByIdQueryHandler: IRequestHandler<GetAvailabilityScheduleByIdQuery, AvailabilityScheduleResponseModel>
{
    private readonly ILogger<GetAvailabilityScheduleByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetAvailabilityScheduleByIdQueryHandler(ILogger<GetAvailabilityScheduleByIdQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<AvailabilityScheduleResponseModel> Handle(GetAvailabilityScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting schedule with Id {id}", request.Id);
        var dbAvailabilitySchedule =
            await _dbContext.AvailabilitySchedules.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbAvailabilitySchedule == null)
        {
            _logger.LogWarning("Schedule with Id {id} not found", request.Id);
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

        _logger.LogInformation("Schedule with Id {id} fetched successfully", response.Id);
        return response;
    }
}