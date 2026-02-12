using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Services.Queries.GetAllServices;

public class GetAllServicesQueryHandler: IRequestHandler<GetAllServicesQuery, PagedResponse<ServiceResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllServicesQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetAllServicesQueryHandler(ILogger<GetAllServicesQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<ServiceResponseModel>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all services. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            PageSize);

        var totalCount = await _dbContext.Services.CountAsync(cancellationToken);

        var dbServices =await  _dbContext.Services
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        var response = dbServices.Select(service => new ServiceResponseModel()
        {
            Id = service.Id,
            CompanyId = service.CompanyId,
            ServiceDescription = service.ServiceDescription,
            ServiceName = service.ServiceName
        }).ToList();
        
        _logger.LogInformation("Fetched {serviceCount} services.", response.Count);

        return new PagedResponse<ServiceResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}