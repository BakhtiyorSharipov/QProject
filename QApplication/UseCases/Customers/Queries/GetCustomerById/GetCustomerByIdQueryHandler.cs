using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler: IRequestHandler<GetCustomerByIdQuery, CustomerResponseModel>
{
    private readonly ILogger<GetCustomerByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    
    public async Task<CustomerResponseModel> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting customer with Id {CustomerId}", request.Id);
        var dbCompany = await _dbContext.Customers.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
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