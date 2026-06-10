using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Companies.Queries.GetAllCompanies;

public class GetAllCompaniesQueryHandler: IRequestHandler<GetAllCompaniesQuery, PagedResponse<CompanyResponseModel>>
{
    private const int PageSize=15;
    private readonly ILogger<GetAllCompaniesQueryHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public GetAllCompaniesQueryHandler(ILogger<GetAllCompaniesQueryHandler> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<CompanyResponseModel>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all companies. PageNumber: {pageNumber}, PageSize: {pageSize}",
            request.PageNumber,
            PageSize);

        var totalCount = await _dbContext.Companies.CountAsync(cancellationToken);

        var dbCompanies = await _dbContext.Companies
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize).ToListAsync(cancellationToken);
        
        var response = dbCompanies.Select(company => new CompanyResponseModel()
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Address = company.Address,
            EmailAddress = company.EmailAddress,
            PhoneNumber = company.PhoneNumber
        }).ToList();


        _logger.LogInformation("Fetched {companyCount} companies.", response.Count);
        
        var pagedResponse= new PagedResponse<CompanyResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };

        return pagedResponse;
    }
}