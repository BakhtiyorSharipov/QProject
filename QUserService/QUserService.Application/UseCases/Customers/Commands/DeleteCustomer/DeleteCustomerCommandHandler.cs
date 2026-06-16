using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QAuthService.Contracts.Events.CustomerEvent;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler: IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteCustomerCommandHandler(ILogger<DeleteCustomerCommandHandler> logger, IUserServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting customer with Id {id}", request.Id);
        var dbCustomer = await _dbContext.Customer.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbCustomer == null)
        {
            _logger.LogWarning("Customer with Id {id} not for deleting.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        _dbContext.Customer.Remove(dbCustomer);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Customer with Id {id} deleted successfully.", request.Id);

        await _publishEndpoint.Publish(new CustomerDeletedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            CustomerId = dbCustomer.Id,
            FirstName = dbCustomer.FirstName,
            LastName = dbCustomer.LastName,
            PhoneNumber = dbCustomer.PhoneNumber
        }, cancellationToken);
        
        return true;
    }
}