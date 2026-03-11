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

namespace QApplication.UseCases.BlockedCustomers.Queries.GetBlockedCustomerById;

public class GetBlockedCustomerByIdQueryHandler: IRequestHandler<GetBlockedCustomerByIdQuery, BlockedCustomerResponseModel>
{
    private readonly ILogger<GetBlockedCustomerByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetBlockedCustomerByIdQueryHandler(ILogger<GetBlockedCustomerByIdQueryHandler> logger, IQueueApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<BlockedCustomerResponseModel> Handle(GetBlockedCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting blocked customer by Id {id}", request.Id);

        var currentEmployee = await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
        var companyId = currentEmployee.CompanyId;
        
        var dbBlockedCustomer =
            await _dbContext.BlockedCustomers
                .Where(s=>s.CompanyId== companyId)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbBlockedCustomer == null)
        {
            _logger.LogInformation("Blocked customer with Id {id} not found.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BlockedCustomerEntity));
        }

        var response = new BlockedCustomerResponseModel()
        {
            Id = dbBlockedCustomer.Id,
            CompanyId = dbBlockedCustomer.CompanyId,
            CustomerId = dbBlockedCustomer.CustomerId,
            BannedUntil = dbBlockedCustomer.BannedUntil,
            DoesBanForever = dbBlockedCustomer.DoesBanForever,
            Reason = dbBlockedCustomer.Reason
        };

        _logger.LogInformation("Blocked customer with Id {id} fetched successfully.", request.Id);
        return response;
    }
}