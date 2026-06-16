using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUserService.Application.Exceptions;
using QUserService.Application.Extensions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Customers.Queries.GetCustomerProfile;

public class GetCustomerProfileQueryHandler: IRequestHandler<GetCustomerProfileQuery, CustomerProfileResponse>
{
    private readonly ILogger<GetCustomerProfileQueryHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetCustomerProfileQueryHandler(ILogger<GetCustomerProfileQueryHandler> logger, IUserServiceApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<CustomerProfileResponse> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting customer profile");
        
        var currentCustomer = await _contextAccessor.CurrentCustomer(_dbContext, cancellationToken);
        var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.CustomerId == currentCustomer.Id, cancellationToken);
        if (user==null)
        {
            _logger.LogWarning("User with customer Id: {customerId} not found", currentCustomer.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"User with CustomerId {currentCustomer.Id} not found");
        }


        var response = new CustomerProfileResponse
        {
            Id = currentCustomer.Id,
            EmailAddress = user.EmailAddress,
            FirstName = currentCustomer.FirstName,
            LastName = currentCustomer.LastName,
            PhoneNumber = currentCustomer.PhoneNumber,
            DateOfBirth = currentCustomer.DateOfBirth,
            Gender = currentCustomer.Gender,
            Country = currentCustomer.Country,
            City = currentCustomer.City,
            Address = currentCustomer.Address,
            PostalCode = currentCustomer.PostalCode,
            CreatedAt = currentCustomer.CreatedAt,
            UpdatedAt = currentCustomer.UpdatedAt
        };

        return response;
    }
}