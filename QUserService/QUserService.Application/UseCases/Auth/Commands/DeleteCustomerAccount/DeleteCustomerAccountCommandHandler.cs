using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QAuthService.Contracts.Events.CustomerEvent;
using QUserService.Application.Extensions;
using QUserService.Application.Interfaces;

namespace QUserService.Application.UseCases.Auth.Commands.DeleteCustomerAccount;

public class DeleteCustomerAccountCommandHandler: IRequestHandler<DeleteCustomerAccountCommand, bool>
{
    private readonly ILogger<DeleteCustomerAccountCommandHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteCustomerAccountCommandHandler(ILogger<DeleteCustomerAccountCommandHandler> logger, IUserServiceApplicationDbContext dbContext, IHttpContextAccessor contextAccessor, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(DeleteCustomerAccountCommand request, CancellationToken cancellationToken)
    {
        
        var user = await _contextAccessor.CurrentUser(_dbContext, cancellationToken);
        var customer = await _contextAccessor.CurrentCustomer(_dbContext, cancellationToken);

        _logger.LogInformation("Deleting customer account {CustomerId}", customer.Id);

        _dbContext.Users.Remove(user);
        _dbContext.Customer.Remove(customer);


        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Customer with Id {id} deleted successfully.", customer.Id);

        await _publishEndpoint.Publish(new CustomerDeletedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            CustomerId = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber
        }, cancellationToken);
        
        return true;
    }
}