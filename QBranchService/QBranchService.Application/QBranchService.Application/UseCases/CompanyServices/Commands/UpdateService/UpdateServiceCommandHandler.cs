using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Contracts.Events.CompanyServiceEvents;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.CompanyServices.Commands.UpdateService;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, CompanyServiceResponseModel>
{
    private readonly ILogger<UpdateServiceCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateServiceCommandHandler(ILogger<UpdateServiceCommandHandler> logger,
        IBranchServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CompanyServiceResponseModel> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating service with Id {id}.", request.Id);

        var dbService = await _dbContext.CompanyServices.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbService == null)
        {
            _logger.LogWarning("Service with Id {id} not found for updating.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyServiceEntity));
        }


        dbService.CompanyId = request.CompanyId;
        dbService.ServiceName = request.ServiceName;
        dbService.ServiceDescription = request.ServiceDescription;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Service with Id {id} updated successfully.", request.Id);

        await _publishEndpoint.Publish(new CompanyServiceUpdatedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            CompanyServiceId = dbService.Id,
            CompanyId = dbService.CompanyId,
            ServiceDescription = dbService.ServiceDescription,
            ServiceName = dbService.ServiceName
        }, cancellationToken);
        
        var response = new CompanyServiceResponseModel()
        {
            Id = dbService.Id,
            CompanyId = dbService.CompanyId,
            ServiceName = dbService.ServiceName,
            ServiceDescription = dbService.ServiceDescription
        };

        return response;
    }
}