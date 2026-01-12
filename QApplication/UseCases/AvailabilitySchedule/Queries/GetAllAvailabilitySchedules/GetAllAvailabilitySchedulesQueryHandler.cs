using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.AvailabilitySchedule.Queries.GetAllAvailabilitySchedules;

public class GetAllAvailabilitySchedulesQueryHandler: IRequestHandler<GetAllAvailabilitySchedulesQuery, PagedResponse<AvailabilityScheduleResponseModel>>
{
    private readonly ILogger<GetAllAvailabilitySchedulesQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetAllAvailabilitySchedulesQueryHandler(ILogger<GetAllAvailabilitySchedulesQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<AvailabilityScheduleResponseModel>> Handle(GetAllAvailabilitySchedulesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all companies. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            request.PageSize);

        var totalCount = await _dbContext.AvailabilitySchedules.CountAsync(cancellationToken);

        var dbAvailabilitySchedule = await _dbContext.AvailabilitySchedules
            .OrderBy(c => c.Id)
            .Skip((request.PageNumber-1) * request.PageSize)
            .Take(request.PageSize).ToListAsync(cancellationToken);
        
        

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
        }).ToList();

        
        _logger.LogInformation("Fetched {companyCount} companies.", response.Count);
        return new PagedResponse<AvailabilityScheduleResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}