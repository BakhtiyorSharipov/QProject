using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Companies.Queries.GetCompanyById;

public class GetCompanyByIdQueryHandler: IRequestHandler<GetCompanyByIdQuery, CompanyResponseModel>
{
    private readonly ILogger<GetCompanyByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetCompanyByIdQueryHandler(ILogger<GetCompanyByIdQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<CompanyResponseModel> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting company with Id {CompanyId}", request.Id);
        var dbCompany = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
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
    }
}