using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Contracts.Events.CompanyEvents;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.Companies.Commands.UpdateCompany;

public class UpdateCompanyCommandHandler: IRequestHandler<UpdateCompanyCommand, CompanyResponseModel>
{
    private readonly ILogger<UpdateCompanyCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateCompanyCommandHandler(ILogger<UpdateCompanyCommandHandler> logger, IBranchServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CompanyResponseModel> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating company with Id {companyId}", request.Id);
        var dbCompany = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbCompany==null)
        {
            _logger.LogWarning("Company with Id {companyId} not found for updating.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }
        
        dbCompany.CompanyName = request.CompanyName;
        dbCompany.Address = request.Address;
        dbCompany.EmailAddress = request.EmailAddress;
        dbCompany.PhoneNumber = request.PhoneNumber;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Company with Id {companyId} updated successfully", request.Id);

        await _publishEndpoint.Publish(new CompanyUpdatedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            CompanyId = dbCompany.Id,
            CompanyName = dbCompany.CompanyName,
            Address = dbCompany.Address,
            EmailAddress = dbCompany.EmailAddress,
            PhoneNumber = dbCompany.PhoneNumber
        }, cancellationToken);

        var response = new CompanyResponseModel()
        {
            Id = dbCompany.Id,
            CompanyName = dbCompany.CompanyName,
            Address = dbCompany.Address,
            EmailAddress = dbCompany.EmailAddress,
            PhoneNumber = dbCompany.PhoneNumber
        };

        return response;
    }
}