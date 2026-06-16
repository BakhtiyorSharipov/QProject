using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUserService.Application.Exceptions;
using QUserService.Application.Extensions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.AvailabilitySchedule.Queries.GetAvailabilityScheduleById;

public class GetAvailabilityScheduleByIdQueryHandler: IRequestHandler<GetAvailabilityScheduleByIdQuery, AvailabilityScheduleResponseModel>
{
    private readonly ILogger<GetAvailabilityScheduleByIdQueryHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetAvailabilityScheduleByIdQueryHandler(ILogger<GetAvailabilityScheduleByIdQueryHandler> logger, IUserServiceApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<AvailabilityScheduleResponseModel> Handle(GetAvailabilityScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting schedule with Id {id}", request.Id);

        var currentEmployee =await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
        var dbAvailabilitySchedule =
            await _dbContext.AvailabilitySchedules
                .Where(s=>s.EmployeeId==currentEmployee.Id)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
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