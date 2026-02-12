using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QContracts.CashingEvents;
using QDomain.Models;

namespace QApplication.UseCases.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler: IRequestHandler<CreateCompanyCommand, CompanyResponseModel>
{
    private readonly ILogger<CreateCompanyCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateCompanyCommandHandler(ILogger<CreateCompanyCommandHandler> logger, IQueueApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
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


        await _publishEndpoint.Publish(new CompanyCacheResetEvent
        {
            OccuredAt = DateTimeOffset.Now,
            CompanyId = company.Id,
        }, cancellationToken);
        
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