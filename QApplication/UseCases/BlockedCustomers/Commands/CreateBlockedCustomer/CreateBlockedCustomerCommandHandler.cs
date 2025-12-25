using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.BlockedCustomers.Commands.CreateBlockedCustomer;

public class CreateBlockedCustomerCommandHandler: IRequestHandler<CreateBlockedCustomerCommand, BlockedCustomerResponseModel>
{
    private readonly ILogger<CreateBlockedCustomerCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public CreateBlockedCustomerCommandHandler(ILogger<CreateBlockedCustomerCommandHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<BlockedCustomerResponseModel> Handle(CreateBlockedCustomerCommand request, CancellationToken cancellationToken)
    {
         _logger.LogInformation("Blocking customer with Id {request.CustomerId}.", request.CustomerId);

         var customer =
             await _dbContext.Customers.FirstOrDefaultAsync(s => s.Id == request.CustomerId, cancellationToken);
        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {request.CustomerId} not found.", request.CustomerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var company = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.CompanyId, cancellationToken);
        if (company == null)
        {
            _logger.LogWarning("Company with Id {request.CompanyId} not found.", request.CompanyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }
        

        var blockedCustomer = new BlockedCustomerEntity()
        {
            CompanyId = request.CompanyId,
            CustomerId = request.CustomerId,
            BannedUntil = request.BannedUntil,
            DoesBanForever = request.DoesBanForever,
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow
        };


        await _dbContext.BlockedCustomers.AddAsync(blockedCustomer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer with Id {request.CustomerId} blocked successfully.", request.CustomerId);

        var response = new BlockedCustomerResponseModel()
        {
            Id = blockedCustomer.Id,
            CompanyId = blockedCustomer.CompanyId,
            CustomerId = blockedCustomer.CustomerId,
            BannedUntil = blockedCustomer.BannedUntil,
            DoesBanForever = blockedCustomer.DoesBanForever,
            Reason = blockedCustomer.Reason
        };

        return response;
    }
}