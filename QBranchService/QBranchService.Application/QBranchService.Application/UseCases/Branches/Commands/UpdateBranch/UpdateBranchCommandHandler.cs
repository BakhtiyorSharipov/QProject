using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.Branches.Commands.UpdateBranch;

public class UpdateBranchCommandHandler: IRequestHandler<UpdateBranchCommand, BranchResponseModel>
{
    private readonly ILogger<UpdateBranchCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public UpdateBranchCommandHandler(ILogger<UpdateBranchCommandHandler> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<BranchResponseModel> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating branch with Id {branchId}", request.Id);
        var branch = await _dbContext.Branches.FirstOrDefaultAsync(s => s.Id == request.Id && s.IsActive == true, cancellationToken);
        if (branch== null)
        {
            _logger.LogInformation("Branch with Id {branchId} not found for updating", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(BranchEntity));
        }

        branch.BranchName = request.BranchName;
        branch.City = request.City;
        branch.Address = request.Address;
        branch.EmailAddress = request.EmailAddress;
        branch.PhoneNumber = request.PhoneNumber;

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Branch with Id {branchId} updated successfully", request.Id);


        var response = new BranchResponseModel
        {
            Id = branch.Id,
            CompanyId = branch.CompanyId,
            BranchName = branch.BranchName,
            City = branch.City,
            Address = branch.Address,
            EmailAddress = branch.EmailAddress,
            PhoneNumber = branch.PhoneNumber,
            CreatedAt = branch.CreatedAt,
            IsActive = branch.IsActive
        };

        return response;

    }
}