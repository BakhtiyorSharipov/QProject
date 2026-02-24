using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Contracts.Requests;
using QBranchService.Contracts.Responses;

namespace QBranchService.Application.Consumers;

public class ValidateCompanyConsumer: IConsumer<CompanyRequest>
{
    private readonly ILogger<ValidateCompanyConsumer> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public ValidateCompanyConsumer(ILogger<ValidateCompanyConsumer> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<CompanyRequest> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("Validating Company {CompanyId} for Request {RequestId}", message.CompanyId, message.RequestId);

        var company = await _dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == message.CompanyId);

        if (company==null)
        {
            _logger.LogWarning("Company {CompanyId} not found", message.CompanyId);
            await context.RespondAsync<CompanyResponse>(new
            {
                message.RequestId,
                message.CompanyId,
                IsValid = false,
                ErrorMessage = "Company not found",
                CompanyName = (string?)null
            });
            
            return;
        }
        
        _logger.LogInformation("Company {CompanyId} validated successfully", message.CompanyId);

        await context.RespondAsync<CompanyResponse>(new
        {
            message.RequestId,
            message.CompanyId,
            IsValid = true,
            ErrorMessage = (string?)null,
            CompanyName = company.CompanyName
        });
    }
}