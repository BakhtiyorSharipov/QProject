using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Contracts.Events.CompanyEvents;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.Companies.Commands.DeleteCompany;

public class DeleteCompanyCommandHandler: IRequestHandler<DeleteCompanyCommand, bool>
{
    private readonly ILogger<DeleteCompanyCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteCompanyCommandHandler(ILogger<DeleteCompanyCommandHandler> logger, IBranchServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting company with Id {companyId}", request.Id);
        var dbCompany = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (dbCompany==null)
        {
            _logger.LogWarning("Company with Id {companyId} not found for deleting", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        _dbContext.Companies.Remove(dbCompany);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new CompanyDeletedEvent
        {

            OccuredAt = DateTimeOffset.UtcNow,
            CompanyId = dbCompany.Id,
            CompanyName = dbCompany.CompanyName,
            Address = dbCompany.Address,
            EmailAddress = dbCompany.EmailAddress,
            PhoneNumber = dbCompany.PhoneNumber
        }, cancellationToken);
        
        _logger.LogInformation("Company with Id {companyId} deleted successfully", request.Id);
        return true;
    }
}