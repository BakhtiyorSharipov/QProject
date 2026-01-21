using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Companies.Queries.GetAllCompanies;

public class GetAllCompaniesQueryHandler: IRequestHandler<GetAllCompaniesQuery, PagedResponse<CompanyResponseModel>>
{
    private readonly ILogger<GetAllCompaniesQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;

    public GetAllCompaniesQueryHandler(ILogger<GetAllCompaniesQueryHandler> logger, IQueueApplicationDbContext dbContext, ICacheService cache)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<PagedResponse<CompanyResponseModel>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all companies. PageNumber: {pageNumber}, PageSize: {pageSize}", request.pageNumber,
            request.pageSize);

        var cacheKey = CacheKeys.AllCompanies(request.pageNumber, request.pageSize);

        var companies = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Cache miss for GetAllCompanies");
            var totalCount = await _dbContext.Companies.CountAsync(cancellationToken);

            var dbCompanies = await _dbContext.Companies
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Take(request.pageSize).ToListAsync(cancellationToken);



            var response = dbCompanies.Select(company => new CompanyResponseModel()
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                Address = company.Address,
                EmailAddress = company.EmailAddress,
                PhoneNumber = company.PhoneNumber
            }).ToList();


            _logger.LogInformation("Fetched {companyCount} companies.", response.Count);
            return new PagedResponse<CompanyResponseModel>
            {
                Items = response,
                PageNumber = request.pageNumber,
                PageSize = request.pageSize,
                TotalCount = totalCount
            };
        }, absoluteExpiration: TimeSpan.FromMinutes(5), cancellationToken: cancellationToken);

        if (companies==null)
        {
            _logger.LogInformation("Data is null. Can not return!");
            throw new Exception("Retrieved data is null");
        }

        return companies;

    }
}