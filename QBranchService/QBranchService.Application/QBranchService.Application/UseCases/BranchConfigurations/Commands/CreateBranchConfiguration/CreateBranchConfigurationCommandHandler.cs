using MediatR;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.BranchConfigurations.Commands.CreateBranchConfiguration;

public class CreateBranchConfigurationCommandHandler: IRequestHandler<CreateBranchConfigurationCommand, BranchConfigurationResponseModel>
{
    private readonly ILogger<BranchConfigurationResponseModel> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public CreateBranchConfigurationCommandHandler(ILogger<BranchConfigurationResponseModel> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<BranchConfigurationResponseModel> Handle(CreateBranchConfigurationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating configurations for BranchId: {BranchId}", request.BranchId);

        var branchConfiguration = new BranchConfigurationEntity
        {
            BranchId = request.BranchId,
            MaxTicketsPerDay = request.MaxTicketsPerDay,
            OpenTime = request.OpenTime,
            CloseTime = request.CloseTime,
            BreakStartTime = request.BreakStartTime,
            BreakEndTime = request.BreakEndTime,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _dbContext.BranchConfigurations.AddAsync(branchConfiguration, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created branch configuration with Id: {branchId}", branchConfiguration.Id);
        
        var response = new BranchConfigurationResponseModel
        {
            Id = branchConfiguration.Id,
            BranchId = branchConfiguration.BranchId,
            MaxTicketsPerDay = branchConfiguration.MaxTicketsPerDay,
            OpenTime = branchConfiguration.OpenTime,
            CloseTime = branchConfiguration.CloseTime,
            BreakStartTime = branchConfiguration.BreakStartTime,
            BreakEndTime = branchConfiguration.BreakEndTime,
            CreatedAt = branchConfiguration.CreatedAt
        };

        return response;
    }
}