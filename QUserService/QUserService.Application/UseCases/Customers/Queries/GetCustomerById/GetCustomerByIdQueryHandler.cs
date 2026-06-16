using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler: IRequestHandler<GetCustomerByIdQuery, CustomerResponseModel>
{
    private readonly ILogger<GetCustomerByIdQueryHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;

    public GetCustomerByIdQueryHandler(ILogger<GetCustomerByIdQueryHandler> logger, IUserServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<CustomerResponseModel> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting customer with Id {CustomerId}", request.Id);
        var dbCompany = await _dbContext.Customer.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbCompany == null)
        {
            _logger.LogWarning("Company with Id {Company} not found", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var response = new CustomerResponseModel()
        {
            Id = dbCompany.Id,
            FirstName = dbCompany.FirstName,
            LastName = dbCompany.LastName,
            PhoneNumber = dbCompany.PhoneNumber
        };

        _logger.LogInformation("Customer with Id {CustomerId} fetched successfully", request.Id);

        return response;
    }
}