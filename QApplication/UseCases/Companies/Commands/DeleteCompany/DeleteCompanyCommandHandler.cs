using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QDomain.Models;

namespace QApplication.UseCase.Companies.Commands.DeleteCompanyCommand;

public class DeleteCompanyCommandHandler: IRequestHandler<DeleteCompanyCommand, bool>
{
    private readonly ILogger<DeleteCompanyCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;
    public DeleteCompanyCommandHandler(ILogger<DeleteCompanyCommandHandler> logger, IQueueApplicationDbContext dbContext, ICacheService cache)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting company with Id {companyId}", request.Id);

        var dbCompany = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken); 
        if (dbCompany == null)
        {
            _logger.LogWarning("Company with Id {companyId} not found for deleting", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        _dbContext.Companies.Remove(dbCompany);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync(CacheKeys.CompanyById(request.Id), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.AllCompanies(1, 10), cancellationToken);
        _logger.LogInformation("Company with Id {companyId} deleted successfully", request.Id);
        return true;
    }
}