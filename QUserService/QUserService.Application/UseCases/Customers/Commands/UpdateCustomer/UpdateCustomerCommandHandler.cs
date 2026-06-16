using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QAuthService.Contracts.Events.CustomerEvent;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler: IRequestHandler<UpdateCustomerCommand, CustomerResponseModel>
{
    private readonly ILogger<UpdateCustomerCommandHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateCustomerCommandHandler(ILogger<UpdateCustomerCommandHandler> logger, IUserServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CustomerResponseModel> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating customer with Id {id}.", request.Id);
        var dbCustomer = await _dbContext.Customer.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);    
        if (dbCustomer == null)
        {
            _logger.LogWarning("Customer with Id {id} not found for updating.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }
        

        dbCustomer.FirstName = request.Firstname;
        dbCustomer.LastName = request.Lastname;
        dbCustomer.PhoneNumber = request.PhoneNumber;
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer with Id {id} updated successfully.", request.Id);

        await _publishEndpoint.Publish(new CustomerUpdatedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            CustomerId = dbCustomer.Id,
            FirstName = dbCustomer.FirstName,
            LastName = dbCustomer.LastName,
            PhoneNumber = dbCustomer.PhoneNumber
        }, cancellationToken);
        
        var response = new CustomerResponseModel()
        {
            Id = dbCustomer.Id,
            FirstName = dbCustomer.FirstName,
            LastName = dbCustomer.LastName,
            PhoneNumber = dbCustomer.PhoneNumber,
        };

        return response;
    }
}