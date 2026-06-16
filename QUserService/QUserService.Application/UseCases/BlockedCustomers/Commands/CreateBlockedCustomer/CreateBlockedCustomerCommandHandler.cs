using System.Net;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QAuthService.Contracts.Events.BlockedCustomerEvent;
using QBranchService.Contracts.Interfaces;
using QBranchService.Contracts.Requests;
using QUserService.Application.Exceptions;
using QUserService.Application.Extensions;
using QUserService.Application.Interfaces;
using QUserService.Application.Responses;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.BlockedCustomers.Commands.CreateBlockedCustomer;

public class CreateBlockedCustomerCommandHandler: IRequestHandler<CreateBlockedCustomerCommand, BlockedCustomerResponseModel>
{
    private readonly ILogger<CreateBlockedCustomerCommandHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IBranchService _branchService;
    private readonly IRequestClient<CompanyRequest> _validationClient;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateBlockedCustomerCommandHandler(ILogger<CreateBlockedCustomerCommandHandler> logger, IUserServiceApplicationDbContext dbContext, IRequestClient<CompanyRequest> validationClient, IHttpContextAccessor contextAccessor, IBranchService branchService, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _validationClient = validationClient;
        _contextAccessor = contextAccessor;
        _branchService = branchService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<BlockedCustomerResponseModel> Handle(CreateBlockedCustomerCommand request, CancellationToken cancellationToken)
    {
         _logger.LogInformation("Blocking customer with Id {request.CustomerId}.", request.CustomerId);

         var customer =
             await _dbContext.Customer.FirstOrDefaultAsync(s => s.Id == request.CustomerId, cancellationToken);
        if (customer == null)
        {
            _logger.LogWarning("Customer with Id {request.CustomerId} not found.", request.CustomerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CustomerEntity));
        }

        var currentEmployee = await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
        var companyId = currentEmployee.CompanyId;

        var validationResponse = await _branchService.CheckCompanyId(new CompanyRequest
        {
            RequestId = Guid.NewGuid(),
            CompanyId = companyId,
            RequestedAt = DateTimeOffset.UtcNow
        });

        if (!validationResponse.IsValid)
        {
            _logger.LogWarning("Company validation failed: {ErrorMessage}", 
                validationResponse.ErrorMessage);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                validationResponse.ErrorMessage ?? "Invalid company");
        }
        
        var existingBlock = await _dbContext.BlockedCustomers
            .FirstOrDefaultAsync(b => 
                    b.CustomerId == request.CustomerId && 
                    b.CompanyId == companyId &&
                    (b.DoesBanForever || b.BannedUntil > DateTime.UtcNow), 
                cancellationToken);

        if (existingBlock != null)
        {
            _logger.LogWarning("Customer {CustomerId} is already blocked for Company {CompanyId}", 
                request.CustomerId, companyId);
            throw new HttpStatusCodeException(HttpStatusCode.Conflict, 
                "Customer is already blocked for this company");
        }
        

        var blockedCustomer = new BlockedCustomerEntity()
        {
            CompanyId = companyId,
            CustomerId = request.CustomerId,
            BannedUntil = request.BannedUntil,
            DoesBanForever = request.DoesBanForever,
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow
        };


        await _dbContext.BlockedCustomers.AddAsync(blockedCustomer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer with Id {request.CustomerId} blocked successfully.", request.CustomerId);

        await _publishEndpoint.Publish(new BlockedCustomerCreatedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            CompanyId = blockedCustomer.CompanyId,
            CustomerId = blockedCustomer.CustomerId,
            BlockedCustomerId = blockedCustomer.Id,
            Reason = blockedCustomer.Reason,
            BannedUntil = blockedCustomer.BannedUntil,
            DoesBanForever = blockedCustomer.DoesBanForever
        }, cancellationToken);

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