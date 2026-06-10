using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Contracts.Events.BranchEvents;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.Branches.Commands.DeleteBranch;

public class DeleteBranchCommandHandler: IRequestHandler<DeleteBranchCommand, bool>
{
    private readonly ILogger<DeleteBranchCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteBranchCommandHandler(ILogger<DeleteBranchCommandHandler> logger, IBranchServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting branch with Id {branchId}", request.Id);
        var branch = await _dbContext.Branches.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (branch==null)
        {
            _logger.LogInformation("Branch with Id {branchId} not found for updating", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BranchEntity));
        }

        branch.IsActive = false;

        await _publishEndpoint.Publish(new BranchDeletedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            BranchId = branch.Id,
            CompanyId = branch.CompanyId,
            BranchName = branch.BranchName,
            City = branch.City,
            Address = branch.Address,
            EmailAddress = branch.EmailAddress,
            PhoneNumber = branch.PhoneNumber,
            IsActive = branch.IsActive
        }, cancellationToken);

        return true;
    }
}