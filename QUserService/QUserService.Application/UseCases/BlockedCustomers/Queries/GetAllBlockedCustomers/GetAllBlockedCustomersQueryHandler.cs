using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUserService.Application.Extensions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.BlockedCustomers.Queries.GetAllBlockedCustomers;

public class GetAllBlockedCustomersQueryHandler: IRequestHandler<GetAllBlockedCustomersQuery, PagedResponse<BlockedCustomerResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllBlockedCustomersQueryHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetAllBlockedCustomersQueryHandler(ILogger<GetAllBlockedCustomersQueryHandler> logger, IUserServiceApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<PagedResponse<BlockedCustomerResponseModel>> Handle(GetAllBlockedCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all blocked customers. PageNumber: {pageNumber}, PageSize: {ageSize}", request.PageNumber,
            PageSize);

        var currentEmployee =await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
        var companyId = currentEmployee.CompanyId;

        var totalCount = await _dbContext.BlockedCustomers
            .Where(s=>s.CompanyId== companyId)
            .CountAsync(cancellationToken);

        var dbBlockedCustomers = await _dbContext.BlockedCustomers
            .Where(s=>s.CompanyId== companyId)
            .OrderBy(c => c.Id)
            .Skip((request.PageNumber-1) * PageSize)
            .Take(PageSize).ToListAsync(cancellationToken);
        
        

        var response = dbBlockedCustomers.Select(blockedCustomer => new BlockedCustomerResponseModel()
        {
            Id = blockedCustomer.Id,
            CompanyId = blockedCustomer.CompanyId,
            CustomerId = blockedCustomer.CustomerId,
            BannedUntil = blockedCustomer.BannedUntil,
            DoesBanForever = blockedCustomer.DoesBanForever,
            Reason = blockedCustomer.Reason
            
        }).ToList();

        
        _logger.LogInformation("Fetched {blockedCustomerCount} blocked customers.", response.Count);
        return new PagedResponse<BlockedCustomerResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}