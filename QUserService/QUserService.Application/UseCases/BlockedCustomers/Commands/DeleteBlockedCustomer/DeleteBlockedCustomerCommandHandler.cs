using System.Net;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QAuthService.Contracts.Events.BlockedCustomerEvent;
using QUserService.Application.Exceptions;
using QUserService.Application.Extensions;
using QUserService.Application.Interfaces;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.BlockedCustomers.Commands.DeleteBlockedCustomer;

public class DeleteBlockedCustomerCommandHandler: IRequestHandler<DeleteBlockedCustomerCommand, bool>
{
    private readonly ILogger<DeleteBlockedCustomerCommandHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteBlockedCustomerCommandHandler(ILogger<DeleteBlockedCustomerCommandHandler> logger, IUserServiceApplicationDbContext dbContext, IHttpContextAccessor contextAccessor, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(DeleteBlockedCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unblocking customer with Id {id}.", request.Id);

        var currentEmployee = await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
        var companyId = currentEmployee.CompanyId;
        
        var dbBlockedCustomer =
            await _dbContext.BlockedCustomers
                .Where(s=>s.CompanyId== companyId)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbBlockedCustomer == null)
        {
            _logger.LogWarning("Blocked customer with Id {id} not found.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BlockedCustomerEntity));
        }

        _dbContext.BlockedCustomers.Remove(dbBlockedCustomer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Blocked customer with Id {id} unblocked successfully.", request.Id);

        await _publishEndpoint.Publish(new BlockedCustomerDeletedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            CompanyId = dbBlockedCustomer.CompanyId,
            CustomerId = dbBlockedCustomer.CustomerId,
            BlockedCustomerId = dbBlockedCustomer.Id,
            Reason = dbBlockedCustomer.Reason,
            BannedUntil = dbBlockedCustomer.BannedUntil,
            DoesBanForever = dbBlockedCustomer.DoesBanForever
        }, cancellationToken);

        return true;
    }
}