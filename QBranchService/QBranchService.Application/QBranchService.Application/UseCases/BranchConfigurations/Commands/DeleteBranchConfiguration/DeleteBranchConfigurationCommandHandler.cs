using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.BranchConfigurations.Commands.DeleteBranchConfiguration;

public class DeleteBranchConfigurationCommandHandler: IRequestHandler<DeleteBranchConfigurationCommand, bool>
{
    private readonly ILogger<DeleteBranchConfigurationCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public DeleteBranchConfigurationCommandHandler(ILogger<DeleteBranchConfigurationCommandHandler> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteBranchConfigurationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting branch configuration with Id: {configurationId}", request.Id);
        var dbBranchConfiguration =
            await _dbContext.BranchConfigurations.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbBranchConfiguration==null)
        {
            _logger.LogError("Branch Configuration with Id: {ConfigurationId} not found", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BranchConfigurationEntity));
        }

        _dbContext.BranchConfigurations.Remove(dbBranchConfiguration);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}