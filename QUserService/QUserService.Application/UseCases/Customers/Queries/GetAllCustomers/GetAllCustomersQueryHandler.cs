using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Customers.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler: IRequestHandler<GetAllCustomersQuery, PagedResponse<CustomerResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllCustomersQueryHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;

    public GetAllCustomersQueryHandler(ILogger<GetAllCustomersQueryHandler> logger, IUserServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    
    public async Task<PagedResponse<CustomerResponseModel>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all customers. PageNumber: {pageNumber}, PageSize: {ageSize}", request.PageNumber,
            PageSize);

        var totalCount = await _dbContext.Customer.CountAsync(cancellationToken);

        var dbCustomers = await _dbContext.Customer
            .OrderBy(c => c.Id)
            .Skip((request.PageNumber-1) * PageSize)
            .Take(PageSize).ToListAsync(cancellationToken);
        
        

        var response = dbCustomers.Select(customer => new CustomerResponseModel()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber
        }).ToList();

        
        _logger.LogInformation("Fetched {customerCount} customers.", response.Count);
        return new PagedResponse<CustomerResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}