using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Requests.CompanyRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCase.Companies.Commands;

public class CreateCompanyCommandHandler: IRequestHandler<CreateCompanyCommand, CompanyResponseModel>
{
    private readonly ILogger<CreateCompanyCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;

    public CreateCompanyCommandHandler(ILogger<CreateCompanyCommandHandler> logger, IQueueApplicationDbContext dbContext, ICacheService cache)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<CompanyResponseModel> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding new company with Name {companyName}", request.CompanyName);

        var company = new CompanyEntity()
        {
            CompanyName = request.CompanyName,
            Address = request.Address,
            EmailAddress = request.EmailAddress,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Companies.AddAsync(company, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(CacheKeys.AllCompanies(1, 10), cancellationToken);
        _logger.LogInformation("Company {companyName} added successfully with Id {companyId}", company.CompanyName,
            company.Id);
        
        var response = new CompanyResponseModel()
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Address = company.Address,
            EmailAddress = company.EmailAddress,
            PhoneNumber = company.PhoneNumber
        };

        return response;
    }
}