using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Contracts.Events.BranchEvents;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.Branches.Commands.CreateBranch;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, BranchResponseModel>
{
    private readonly ILogger<CreateBranchCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateBranchCommandHandler(ILogger<CreateBranchCommandHandler> logger,
        IBranchServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<BranchResponseModel> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating branch for CompanyId: {CompanyId}", request.CompanyId);

        var company = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.CompanyId, cancellationToken);
        if (company == null)
        {
            _logger.LogError("Company with Id:{CompanyId} not found!", company.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        var branch = new BranchEntity
        {
            CompanyId = request.CompanyId,
            BranchName = request.BranchName,
            City = request.City,
            Address = request.Address,
            EmailAddress = request.EmailAddress,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTimeOffset.UtcNow,
            IsActive = true
        };

        
        await _dbContext.Branches.AddAsync(branch, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Branch {branchName} added successfully with Id {branchId}", branch.BranchName,
            branch.Id);

        await _publishEndpoint.Publish(new BranchCreatedEvent
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
        
        
        var response = new BranchResponseModel
        {
            Id = branch.Id,
            CompanyId = branch.CompanyId,
            BranchName = branch.BranchName,
            City = branch.City,
            Address = branch.Address,
            EmailAddress = branch.EmailAddress,
            PhoneNumber = branch.PhoneNumber,
            CreatedAt = DateTimeOffset.UtcNow,
            IsActive = true
        };

        return response;
    }
}