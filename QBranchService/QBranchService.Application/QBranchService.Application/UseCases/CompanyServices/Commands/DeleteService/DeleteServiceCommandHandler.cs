using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Contracts.Events.CompanyServiceEvents;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.CompanyServices.Commands.DeleteService;

public class DeleteServiceCommandHandler: IRequestHandler<DeleteServiceCommand, bool>
{
    private readonly ILogger<DeleteServiceCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteServiceCommandHandler(ILogger<DeleteServiceCommandHandler> logger, IBranchServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting service with Id {id}", request.Id);
        var dbService = await _dbContext.CompanyServices.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbService == null)
        {
            _logger.LogWarning("Service with Id {id} not found for deleting.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyServiceEntity));
        }

        _dbContext.CompanyServices.Remove(dbService);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Service with Id {id} deleted successfully.", request.Id);

        await _publishEndpoint.Publish(new CompanyServiceDeletedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            CompanyServiceId = dbService.Id,
            CompanyId = dbService.CompanyId,
            ServiceDescription = dbService.ServiceDescription,
            ServiceName = dbService.ServiceName
        }, cancellationToken);
        
        return true;
    }
}