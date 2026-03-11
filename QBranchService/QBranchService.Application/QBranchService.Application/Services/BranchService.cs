using MagicOnion;
using MagicOnion.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Contracts.Interfaces;
using QBranchService.Contracts.Requests;
using QBranchService.Contracts.Responses;

namespace QBranchService.Application.Services;

public class BranchService : ServiceBase<IBranchService>, IBranchService
{
    private readonly ILogger<BranchService> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public BranchService(ILogger<BranchService> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async UnaryResult<BranchResponse> CheckBranchId(BranchRequest request)
    {
        _logger.LogInformation("Checking Branch for Request {RequestId}: BranchId: {BranchId}", request.RequestId,
            request.BranchId);

        var branch = await _dbContext.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.BranchId
                                      && s.CompanyId == request.CompanyId
                                      && s.IsActive);

        if (branch == null)
        {
            return new BranchResponse
            {
                RequestId = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                BranchId = request.BranchId,
                IsValid = false,
                BranchName = null,
                ErrorMessage = "Branch not found"
            };
        }

        return new BranchResponse
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            BranchId = request.BranchId,
            IsValid = true,
            BranchName = branch.BranchName,
            ErrorMessage = null
        };
    }

    public async UnaryResult<CompanyResponse> CheckCompanyId(CompanyRequest request)
    {
        _logger.LogInformation("Checking Company for Request {RequestId}: CompanyId: {CompanyId}", request.RequestId,
            request.CompanyId);

        var company = await _dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.CompanyId);

        if (company == null)
        {
            return new CompanyResponse
            {
                RequestId = request.RequestId,
                CompanyId = request.CompanyId,
                IsValid = false,
                CompanyName = null,
                ErrorMessage = "Company not found"
            };
        }

        return new CompanyResponse
        {
            RequestId = request.RequestId,
            CompanyId = request.CompanyId,
            IsValid = true,
            CompanyName = company.CompanyName,
            ErrorMessage = null
        };
    }

    public async UnaryResult<CompanyServiceResponse> CheckCompanyServiceId(CompanyServiceRequest request)
    {
        _logger.LogInformation("Checking CompanyService for Request {RequestId}: CompanyServiceId: {CompanyServiceId}"
            , request.RequestId, request.CompanyServiceId);

        var companyService = await _dbContext.CompanyServices
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.CompanyServiceId
                                      && s.CompanyId == request.CompanyId);

        if (companyService == null)
        {
            return new CompanyServiceResponse
            {
                RequestId = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                CompanyServiceId = request.CompanyServiceId,
                IsValid = false,
                ErrorMessage = "CompanyService not found",
                CompanyServiceName = null
            };
        }

        return new CompanyServiceResponse
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            CompanyServiceId = request.CompanyServiceId,
            IsValid = true,
            ErrorMessage = null,
            CompanyServiceName = companyService.ServiceName
        };
    }

public async UnaryResult<QueueCreationValidationResponse> ValidateQueueCreationAsync(
    QueueCreationValidationRequest request)
{
    _logger.LogInformation("Validating queue creation for Branch {BranchId} at {StartTime}", 
        request.BranchId, request.RequestedStartTime);

    var requestTime = TimeOnly.FromDateTime(request.RequestedStartTime.DateTime);

    
    var branchConfig = await _dbContext.BranchConfigurations
        .FirstOrDefaultAsync(bc => bc.BranchId == request.BranchId);

   
    if (branchConfig == null)
    {
        return new QueueCreationValidationResponse
        {
            RequestId = request.RequestId,
            IsValid = false,
            ErrorMessage = "Branch configuration not found"
        };
    }

    
    var isWithinWorkingHours = requestTime >= branchConfig.OpenTime && 
                               requestTime <= branchConfig.CloseTime;
    
    string? workingHoursMessage = null;
    if (!isWithinWorkingHours)
    {
        workingHoursMessage = $"Branch hours: {branchConfig.OpenTime:HH:mm} - {branchConfig.CloseTime:HH:mm}";
    }

    
    var isWithinBreakTime = false;
    string? breakTimeMessage = null;
    
    if (branchConfig.BreakStartTime.HasValue && branchConfig.BreakEndTime.HasValue)
    {
        isWithinBreakTime = requestTime >= branchConfig.BreakStartTime && 
                            requestTime <= branchConfig.BreakEndTime;
        
        if (isWithinBreakTime)
        {
            breakTimeMessage = $"Branch on break: {branchConfig.BreakStartTime:HH:mm} - {branchConfig.BreakEndTime:HH:mm}";
        }
    }

    
    var isValid = isWithinWorkingHours && !isWithinBreakTime;

    var response = new QueueCreationValidationResponse
    {
        RequestId = request.RequestId,
        IsValid = isValid,
        
        
        IsWithinWorkingHours = isWithinWorkingHours,
        WorkingHoursMessage = workingHoursMessage,
        
        
        IsWithinBreakTime = isWithinBreakTime,
        BreakTimeMessage = breakTimeMessage,
        
        
        MaxTicketsPerDay = branchConfig.MaxTicketsPerDay
    };

    
    if (!isValid)
    {
        var errors = new List<string>();
        if (!isWithinWorkingHours) errors.Add(workingHoursMessage);
        if (isWithinBreakTime) errors.Add(breakTimeMessage);
        
        response.ErrorMessage = string.Join("; ", errors);
    }

    return response;
}
}