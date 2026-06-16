using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUserService.Application.Extensions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.AvailabilitySchedule.Queries.GetAllAvailabilitySchedules;

public class GetAllAvailabilitySchedulesQueryHandler: IRequestHandler<GetAllAvailabilitySchedulesQuery, PagedResponse<AvailabilityScheduleResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllAvailabilitySchedulesQueryHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetAllAvailabilitySchedulesQueryHandler(ILogger<GetAllAvailabilitySchedulesQueryHandler> logger, IUserServiceApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<PagedResponse<AvailabilityScheduleResponseModel>> Handle(GetAllAvailabilitySchedulesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all companies. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            PageSize);

        var currentEmployee = await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
        var totalCount = await _dbContext.AvailabilitySchedules
            .Where(s=>s.EmployeeId== currentEmployee.Id)
            .CountAsync(cancellationToken);

        var dbAvailabilitySchedule = await _dbContext.AvailabilitySchedules
            .Where(s=>s.EmployeeId== currentEmployee.Id)
            .OrderBy(c => c.Id)
            .Skip((request.PageNumber-1) * PageSize)
            .Take(PageSize).ToListAsync(cancellationToken);
        
        

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
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}