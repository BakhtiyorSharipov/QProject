using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.BranchConfigurations.Queries.GetBranchConfigurationById;

public class GetBranchConfigurationByIdQueryHandler: IRequestHandler<GetBranchConfigurationByIdQuery, BranchConfigurationResponseModel>
{
    private readonly ILogger<GetBranchConfigurationByIdQueryHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public GetBranchConfigurationByIdQueryHandler(ILogger<GetBranchConfigurationByIdQueryHandler> logger,
        IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<BranchConfigurationResponseModel> Handle(GetBranchConfigurationByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting branch configuration with Id: {ConfigurationId}", request.Id);
        var dbBranchConfiguration =
            await _dbContext.BranchConfigurations.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbBranchConfiguration==null)
        {
            _logger.LogError("Branch Configuration with Id: {ConfigurationId} not found", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BranchConfigurationEntity));
        }

        var response = new BranchConfigurationResponseModel
        {
            Id = dbBranchConfiguration.Id,
            BranchId = dbBranchConfiguration.BranchId,
            MaxTicketsPerDay = dbBranchConfiguration.MaxTicketsPerDay,
            OpenTime = dbBranchConfiguration.OpenTime,
            CloseTime = dbBranchConfiguration.CloseTime,
            BreakStartTime = dbBranchConfiguration.BreakStartTime,
            BreakEndTime = dbBranchConfiguration.BreakEndTime,
            CreatedAt = dbBranchConfiguration.CreatedAt
        };

        return response;
    }
}