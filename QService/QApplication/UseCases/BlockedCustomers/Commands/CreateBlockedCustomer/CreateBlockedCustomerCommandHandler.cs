using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Messages;
using QApplication.Responses;
using QBranchService.Contracts.Requests;
using QBranchService.Contracts.Responses;
using QDomain.Models;

namespace QApplication.UseCases.BlockedCustomers.Commands.CreateBlockedCustomer;

public class CreateBlockedCustomerCommandHandler: IRequestHandler<CreateBlockedCustomerCommand, BlockedCustomerResponseModel>
{
    private readonly ILogger<CreateBlockedCustomerCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IRequestClient<CompanyRequest> _validationClient;

    public CreateBlockedCustomerCommandHandler(ILogger<CreateBlockedCustomerCommandHandler> logger, IQueueApplicationDbContext dbContext, IRequestClient<CompanyRequest> validationClient)
    {
        _logger = logger;
        _dbContext = dbContext;
        _validationClient = validationClient;
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


        var validationResponse = await _validationClient.GetResponse<CompanyResponse>(new ValidateCompanyMessage
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            RequestedAt = DateTimeOffset.UtcNow
        }, cancellationToken, RequestTimeout.After(s:5));

        if (!validationResponse.Message.IsValid)
        {
            _logger.LogWarning("Company validation failed: {ErrorMessage}", 
                validationResponse.Message.ErrorMessage);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                validationResponse.Message.ErrorMessage ?? "Invalid company");
        }
        
        var existingBlock = await _dbContext.BlockedCustomers
            .FirstOrDefaultAsync(b => 
                    b.CustomerId == request.CustomerId && 
                    b.CompanyId == request.CompanyId &&
                    (b.DoesBanForever || b.BannedUntil > DateTime.UtcNow), 
                cancellationToken);

        if (existingBlock != null)
        {
            _logger.LogWarning("Customer {CustomerId} is already blocked for Company {CompanyId}", 
                request.CustomerId, request.CompanyId);
            throw new HttpStatusCodeException(HttpStatusCode.Conflict, 
                "Customer is already blocked for this company");
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