using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.Branches.Queries.GetBranchById;

public class GetBranchByIdQueryHandler: IRequestHandler<GetBranchByIdQuery, BranchResponseModel>
{
    private readonly ILogger<GetBranchByIdQueryHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public GetBranchByIdQueryHandler(ILogger<GetBranchByIdQueryHandler> logger,
        IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<BranchResponseModel> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting branch with Id: {branchId}", request.Id);
        var dbBranch = await _dbContext.Branches.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbBranch==null)
        {
            _logger.LogError("Branch with Id: {branchId} not found", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BranchEntity));
        }
        
        var response = new BranchResponseModel()
        {
            Id = dbBranch.Id,
            CompanyId = dbBranch.CompanyId,
            BranchName = dbBranch.BranchName,
            City = dbBranch.City,
            Address = dbBranch.Address,
            EmailAddress = dbBranch.EmailAddress,
            PhoneNumber = dbBranch.PhoneNumber,
            CreatedAt = dbBranch.CreatedAt,
            IsActive = dbBranch.IsActive
        };

        _logger.LogInformation("Branch with Id {BranchId} fetched successfully", request.Id);

        return response;
    }
}