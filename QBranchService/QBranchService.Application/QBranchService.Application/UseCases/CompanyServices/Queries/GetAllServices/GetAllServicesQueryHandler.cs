using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Responses;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Application.UseCases.CompanyServices.Queries.GetAllServices;

namespace QApplication.UseCases.Services.Queries.GetAllServices;

public class GetAllServicesQueryHandler: IRequestHandler<GetAllServicesQuery, PagedResponse<CompanyServiceResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllServicesQueryHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public GetAllServicesQueryHandler(ILogger<GetAllServicesQueryHandler> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<CompanyServiceResponseModel>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all services. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            PageSize);

        var totalCount = await _dbContext.CompanyServices.CountAsync(cancellationToken);

        var dbServices =await  _dbContext.CompanyServices
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        var response = dbServices.Select(service => new CompanyServiceResponseModel()
        {
            Id = service.Id,
            CompanyId = service.CompanyId,
            ServiceDescription = service.ServiceDescription,
            ServiceName = service.ServiceName
        }).ToList();
        
        _logger.LogInformation("Fetched {serviceCount} services.", response.Count);

        return new PagedResponse<CompanyServiceResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}