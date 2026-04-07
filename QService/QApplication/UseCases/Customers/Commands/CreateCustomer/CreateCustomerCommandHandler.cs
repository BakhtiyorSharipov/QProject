using System.Net;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Requests.CustomerRequest;
using QApplication.Responses;
using QContracts.Events.CustomerEvent;
using QDomain.Models;

namespace QApplication.UseCases.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler: IRequestHandler<CreateCustomerCommand, CustomerResponseModel>
{
    private readonly ILogger<CreateCustomerCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateCustomerCommandHandler(ILogger<CreateCustomerCommandHandler> logger, IQueueApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CustomerResponseModel> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding new customer with name {request.FirstName}", request.Firstname);

        var customer = new CustomerEntity()
        {
            FirstName = request.Firstname,
            LastName = request.Lastname,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Customers.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer {customer.FirstName} added successfully with Id {customer.Id}",
            customer.FirstName, customer.Id);

        await _publishEndpoint.Publish(new CustomerCreatedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            CustomerId = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber
        }, cancellationToken);
        
        var response = new CustomerResponseModel()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
        };

        return response;
    }
}