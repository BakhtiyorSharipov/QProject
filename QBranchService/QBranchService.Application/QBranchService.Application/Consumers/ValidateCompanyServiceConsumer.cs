using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Contracts.Requests;
using QBranchService.Contracts.Responses;

namespace QBranchService.Application.Consumers;

public class ValidateCompanyServiceConsumer: IConsumer<CompanyServiceRequest>
{
    private readonly ILogger<ValidateCompanyServiceConsumer> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public ValidateCompanyServiceConsumer(ILogger<ValidateCompanyServiceConsumer> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyServiceRequest> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("Validating CompanyService {CompanyServiceId} for Request {RequestId}", message.CompanyServiceId, message.RequestId);

        var companyService = await _dbContext.CompanyServices
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == message.CompanyServiceId);

        if (companyService==null)
        {
            _logger.LogWarning("CompanyService {CompanyServiceId} not found", message.CompanyServiceId);
            await context.RespondAsync<CompanyServiceResponse>(new
            {
                message.RequestId,
                message.CompanyServiceId,
                IsValid = false,
                ErrorMessage = "CompanyService not found",
                CompanyName = (string?)null
            });
            
            return;
        }
        
        _logger.LogInformation("CompanyService {CompanyServiceId} validated successfully", message.CompanyServiceId);

        await context.RespondAsync<CompanyServiceResponse>(new
        {
            message.RequestId,
            message.CompanyServiceId,
            IsValid = true,
            ErrorMessage = (string?)null,
            CompanyServiceName = companyService.ServiceName
        });
    }
}