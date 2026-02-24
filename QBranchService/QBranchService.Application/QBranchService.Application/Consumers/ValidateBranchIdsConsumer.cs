using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Contracts.Requests;
using QBranchService.Contracts.Responses;

namespace QBranchService.Application.Consumers;

public class ValidateBranchIdsConsumer : IConsumer<BranchIdsRequest>
{
    private readonly ILogger<ValidateBranchIdsConsumer> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public ValidateBranchIdsConsumer(ILogger<ValidateBranchIdsConsumer> logger,
        IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BranchIdsRequest> context)
    {
        var message = context.Message;
        _logger.LogInformation(
            "Validating IDs for Request {RequestId}: Company {CompanyId}, Branch {BranchId},CompanyService {ServiceId}",
            message.RequestId, message.CompanyId, message.BranchId, message.CompanyServiceId);

        var company = await _dbContext.Companies.AnyAsync(s => s.Id == message.CompanyId);
        if (!company)
        {
            await ResponseInvalid(context, message, "Company not found");
            return;
        }

        var branch = await _dbContext.Branches.FirstOrDefaultAsync(s => s.Id == message.BranchId &&
                                                                        s.CompanyId == message.CompanyId &&
                                                                        s.IsActive);

        if (branch==null)
        {
            await ResponseInvalid(context, message, "Branch not found");
        }

        var service = await _dbContext.CompanyServices.FirstOrDefaultAsync(s => s.Id == message.CompanyServiceId &&
            s.CompanyId == message.CompanyId);

        if (service==null)
        {
            await ResponseInvalid(context, message, "CompanyService not found for this company");
        }
        
        _logger.LogInformation("Validation successful for Request {RequestId}", message.RequestId);

        await context.RespondAsync<BranchIdsResponse>(new
        {
            message.RequestId,
            message.CompanyId,
            message.BranchId,
            message.CompanyServiceId,
            IsValid = true,
            ErrorMessage = (string?)null
        });
    }

    private static async Task ResponseInvalid(ConsumeContext<BranchIdsRequest> context, BranchIdsRequest message,
        string errorMessage)
    {
        await context.RespondAsync<BranchIdsResponse>(new
        {
            message.RequestId,
            message.CompanyId,
            message.BranchId,
            message.CompanyServiceId,
            IsValid = false,
            ErrorMessage = errorMessage
        });
    }
}