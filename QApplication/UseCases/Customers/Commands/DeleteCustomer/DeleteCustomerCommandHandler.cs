using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QDomain.Models;

namespace QApplication.UseCases.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler: IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting customer with Id {id}", request.Id);
        var dbCustomer = await _dbContext.Customers.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbCustomer == null)
        {
            _logger.LogWarning("Customer with Id {id} not for deleting.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        _dbContext.Customers.Remove(dbCustomer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer with Id {id} deleted successfully.", request.Id);

        return true;
    }
}