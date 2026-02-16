using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Companies.Queries.GetAllCompanies;

public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, PagedResponse<CompanyResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllCompaniesQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;

    public GetAllCompaniesQueryHandler(ILogger<GetAllCompaniesQueryHandler> logger,
        IQueueApplicationDbContext dbContext, ICacheService cache)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<PagedResponse<CompanyResponseModel>> Handle(GetAllCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all companies. PageNumber: {pageNumber}, PageSize: {pageSize}",
            request.PageNumber,
            PageSize);

        var hashKey = CacheKeys.AllCompaniesKey;
        var filed = CacheKeys.AllCompaniesFiled(request.PageNumber);

        var cached = await _cache.HashGetAsync<PagedResponse<CompanyResponseModel>>(hashKey, filed, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var totalCount = await _dbContext.Companies.CountAsync(cancellationToken);

        var dbCompanies = await _dbContext.Companies
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize).ToListAsync(cancellationToken);


        var response = dbCompanies.Select(company => new CompanyResponseModel()
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Address = company.Address,
            EmailAddress = company.EmailAddress,
            PhoneNumber = company.PhoneNumber
        }).ToList();


        _logger.LogInformation("Fetched {companyCount} companies.", response.Count);
        
        var pagedResponse= new PagedResponse<CompanyResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };

        await _cache.HashSetAsync(hashKey, filed, pagedResponse, TimeSpan.FromMinutes(10), cancellationToken);

        return pagedResponse;
    }
}