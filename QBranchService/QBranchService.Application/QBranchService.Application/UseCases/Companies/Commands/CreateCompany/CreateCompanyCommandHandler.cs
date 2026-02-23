using MediatR;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyResponseModel>
{
    private readonly ILogger<CreateCompanyCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;


    public CreateCompanyCommandHandler(ILogger<CreateCompanyCommandHandler> logger,
        IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
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
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _dbContext.Companies.AddAsync(company, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Company {companyName} added successfully with Id {companyId}", company.CompanyName,
            company.Id);

        var response = new CompanyResponseModel
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Address = company.Address,
            EmailAddress = company.EmailAddress,
            PhoneNumber = company.PhoneNumber,
            CreatedAt = company.CreatedAt
        };

        return response;
    }
}