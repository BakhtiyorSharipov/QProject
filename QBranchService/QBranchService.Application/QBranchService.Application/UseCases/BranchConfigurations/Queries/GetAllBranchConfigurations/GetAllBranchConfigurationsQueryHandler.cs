using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.BranchConfigurations.Queries.GetAllBranchConfigurations;

public class GetAllBranchConfigurationsQueryHandler : IRequestHandler<GetAllBranchConfigurationsQuery,
    PagedResponse<BranchConfigurationResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllBranchConfigurationsQueryHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public GetAllBranchConfigurationsQueryHandler(ILogger<GetAllBranchConfigurationsQueryHandler> logger,
        IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<BranchConfigurationResponseModel>> Handle(GetAllBranchConfigurationsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all companies. PageNumber: {pageNumber}, PageSize: {pageSize}",
            request.PageNumber,
            PageSize);

        var total = await _dbContext.BranchConfigurations.CountAsync(cancellationToken);

        var dbBranchConfiguration = await _dbContext.BranchConfigurations
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        var response = dbBranchConfiguration.Select(s => new BranchConfigurationResponseModel
        {
            Id = s.Id,
            BranchId = s.BranchId,
            MaxTicketsPerDay = s.MaxTicketsPerDay,
            OpenTime = s.OpenTime,
            CloseTime = s.CloseTime,
            BreakStartTime = s.BreakStartTime,
            BreakEndTime = s.BreakEndTime,
            CreatedAt = s.CreatedAt
        }).ToList();

        _logger.LogInformation("Fetched {configurationCount} branch configurations.", response.Count);

        var pagedResponse = new PagedResponse<BranchConfigurationResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = total
        };

        return pagedResponse;
    }
}