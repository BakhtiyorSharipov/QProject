using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.BranchConfigurations.Commands.UpdateBranchConfiguration;

public class
    UpdateBranchConfigurationCommandHandler : IRequestHandler<UpdateBranchConfigurationCommand,
    BranchConfigurationResponseModel>
{
    private readonly ILogger<UpdateBranchConfigurationCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public UpdateBranchConfigurationCommandHandler(ILogger<UpdateBranchConfigurationCommandHandler> logger,
        IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<BranchConfigurationResponseModel> Handle(UpdateBranchConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating branch configuration with Id {configurationId}", request.Id);
        var dbBranchConfiguration =
            await _dbContext.BranchConfigurations.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbBranchConfiguration == null)
        {
            _logger.LogError("Branch Configuration with Id: {ConfigurationId} not found", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BranchConfigurationEntity));
        }

        dbBranchConfiguration.MaxTickets = request.MaxTickest;
        dbBranchConfiguration.OpenTime = request.OpenTime;
        dbBranchConfiguration.CloseTime = request.CloseTime;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Branch Configuration with Id {configurationId} updated successfully", request.Id);

        var response = new BranchConfigurationResponseModel
        {
            Id = dbBranchConfiguration.Id,
            BranchId = dbBranchConfiguration.BranchId,
            MaxTickets = dbBranchConfiguration.MaxTickets,
            OpenTime = dbBranchConfiguration.OpenTime,
            CloseTime = dbBranchConfiguration.CloseTime,
            CreatedAt = dbBranchConfiguration.CreatedAt
        };

        return response;
    }
}