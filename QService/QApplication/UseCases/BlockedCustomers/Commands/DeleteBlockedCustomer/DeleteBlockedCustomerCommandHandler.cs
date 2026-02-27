using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Extensions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.BlockedCustomers.Commands.DeleteBlockedCustomer;

public class DeleteBlockedCustomerCommandHandler: IRequestHandler<DeleteBlockedCustomerCommand, bool>
{
    private readonly ILogger<DeleteBlockedCustomerCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public DeleteBlockedCustomerCommandHandler(ILogger<DeleteBlockedCustomerCommandHandler> logger, IQueueApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
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

        return true;
    }
}