using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using Serilog;

namespace QApplication.UseCases.Customers.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler: IRequestHandler<GetAllCustomersQuery, PagedResponse<CustomerResponseModel>>
{

    private readonly ILogger<GetAllCustomersQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetAllCustomersQueryHandler(ILogger<GetAllCustomersQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    
    public async Task<PagedResponse<CustomerResponseModel>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all customers. PageNumber: {pageNumber}, PageSize: {ageSize}", request.pageNumber,
            request.pageSize);

        var totalCount = await _dbContext.Customers.CountAsync(cancellationToken);

        var dbCustomers = await _dbContext.Customers
            .OrderBy(c => c.Id)
            .Skip((request.pageNumber-1) * request.pageSize)
            .Take(request.pageSize).ToListAsync(cancellationToken);
        
        

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
            PageNumber = request.pageNumber,
            PageSize = request.pageSize,
            TotalCount = totalCount
        };
    }
}