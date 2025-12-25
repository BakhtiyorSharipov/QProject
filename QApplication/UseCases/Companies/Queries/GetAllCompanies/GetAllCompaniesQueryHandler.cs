using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Companies.Queries.GetAllCompanies;

public class GetAllCompaniesQueryHandler: IRequestHandler<GetAllCompaniesQuery, PagedResponse<CompanyResponseModel>>
{
    private readonly ILogger<GetAllCompaniesQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetAllCompaniesQueryHandler(ILogger<GetAllCompaniesQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<CompanyResponseModel>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all companies. PageNumber: {pageNumber}, PageSize: {pageSize}", request.pageNumber,
            request.pageSize);

        var totalCount = await _dbContext.Companies.CountAsync(cancellationToken);

        var dbCompanies = await _dbContext.Companies
            .OrderBy(c => c.Id)
            .Skip((request.pageNumber-1) * request.pageSize)
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
        
    }
}