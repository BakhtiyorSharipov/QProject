using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Branches.Queries.GetAllBranches;

public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, PagedResponse<BranchResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllBranchesQueryHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public GetAllBranchesQueryHandler(ILogger<GetAllBranchesQueryHandler> logger,
        IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<BranchResponseModel>> Handle(GetAllBranchesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all companies. PageNumber: {pageNumber}, PageSize: {pageSize}",
            request.PageNumber,
            PageSize);

        var total = await _dbContext.Branches.CountAsync(cancellationToken);
        
        var dbBranches = await _dbContext.Branches
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize).ToListAsync(cancellationToken);

        var response = dbBranches.Select(s => new BranchResponseModel
        {
            Id = s.Id,
            CompanyId = s.CompanyId,
            BranchName = s.BranchName,
            City = s.City,
            Address = s.Address,
            EmailAddress = s.EmailAddress,
            PhoneNumber = s.PhoneNumber,
            CreatedAt = s.CreatedAt,
            IsActive = s.IsActive
        }).ToList();
        
        _logger.LogInformation("Fetched {branchCount} branches.", response.Count);

        var pagedResponse = new PagedResponse<BranchResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = total
        };

        return pagedResponse;

    }
}