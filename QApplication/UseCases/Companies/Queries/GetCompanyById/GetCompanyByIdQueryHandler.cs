using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Companies.Queries.GetCompanyById;

public class GetCompanyByIdQueryHandler: IRequestHandler<GetCompanyByIdQuery, CompanyResponseModel>
{
    private readonly ILogger<GetCompanyByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;

    public GetCompanyByIdQueryHandler(ILogger<GetCompanyByIdQueryHandler> logger, IQueueApplicationDbContext dbContext, ICacheService cache)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<CompanyResponseModel> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting company with Id {CompanyId}", request.Id);

        var cacheKey = CacheKeys.CustomerById(request.Id);

        var company = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Cache miss for Company {Id}", request.Id);
            var dbCompany = await _dbContext.Companies
                .AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            
            if (dbCompany == null)
            {
                _logger.LogWarning("Company with Id {Company} not found", request.Id);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            } 
            
            var response = new CompanyResponseModel()
            {
                Id = dbCompany.Id,
                CompanyName = dbCompany.CompanyName,
                Address = dbCompany.Address,
                EmailAddress = dbCompany.EmailAddress,
                PhoneNumber = dbCompany.PhoneNumber
            };

            _logger.LogInformation("Company with Id {CompanyId} fetched successfully", request.Id);

            return response;
        }, absoluteExpiration: TimeSpan.FromMinutes(10), cancellationToken: cancellationToken);


        if (company == null)
        {
            _logger.LogInformation("Data is null. Can not return!");
            throw new Exception("Retrieved data is null");
        }
        
        return company;
    }
}