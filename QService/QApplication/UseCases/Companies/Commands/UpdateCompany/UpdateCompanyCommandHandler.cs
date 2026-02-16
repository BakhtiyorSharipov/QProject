using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QContracts.CashingEvents;
using QDomain.Models;

namespace QApplication.UseCases.Companies.Commands.UpdateCompany;

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, CompanyResponseModel>
{
    private readonly ILogger<UpdateCompanyCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateCompanyCommandHandler(ILogger<UpdateCompanyCommandHandler> logger,
        IQueueApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CompanyResponseModel> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating company with Id {companyId}", request.Id);
        var dbCompany = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbCompany == null)
        {
            _logger.LogWarning("Company with Id {companyId} not found for updating.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }


        dbCompany.CompanyName = request.CompanyName;
        dbCompany.Address = request.Address;
        dbCompany.EmailAddress = request.EmailAddress;
        dbCompany.PhoneNumber = request.PhoneNumber;


        await _dbContext.SaveChangesAsync(cancellationToken);
        await _publishEndpoint.Publish(new CompanyCacheResetEvent
        {
            OccuredAt = DateTimeOffset.Now,
            CompanyId = dbCompany.Id
        }, cancellationToken);

        _logger.LogInformation("Company with Id {companyId} updated successfully", request.Id);

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