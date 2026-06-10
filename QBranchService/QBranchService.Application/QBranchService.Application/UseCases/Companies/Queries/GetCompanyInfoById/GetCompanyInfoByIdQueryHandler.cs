using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.Companies.Queries.GetCompanyInfoById;

public class GetCompanyInfoByIdQueryHandler : IRequestHandler<GetCompanyInfoByIdQuery, CompanyByIdResponseModel>
{
    private readonly ILogger<GetCompanyInfoByIdQueryHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public GetCompanyInfoByIdQueryHandler(ILogger<GetCompanyInfoByIdQueryHandler> logger,
        IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<CompanyByIdResponseModel> Handle(GetCompanyInfoByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting company with Id {CompanyId}", request.Id);

        var dbCompany = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbCompany == null)
        {
            _logger.LogError("Company with Id {Company} not found", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        var companyBranches = await _dbContext.Branches
            .Where(s => s.CompanyId == dbCompany.Id)
            .ToListAsync(cancellationToken);

        var companyService = await _dbContext.CompanyServices
            .Where(s => s.CompanyId == dbCompany.Id)
            .ToListAsync(cancellationToken);


        var branchResponseList = new List<CompanyBranchesResponseModel>();
        foreach (var companyBranch in companyBranches)
        {
            var branchConfiguration =
                await _dbContext.BranchConfigurations.FirstOrDefaultAsync(s => s.BranchId == companyBranch.Id,
                    cancellationToken);

            if (branchConfiguration != null)
            {
                branchResponseList.Add(new CompanyBranchesResponseModel
                {
                    Id = companyBranch.Id,
                    CompanyId = companyBranch.CompanyId,
                    BranchName = companyBranch.BranchName,
                    City = companyBranch.City,
                    Address = companyBranch.Address,
                    PhoneNumber = companyBranch.PhoneNumber,
                    EmailAddress = companyBranch.EmailAddress,
                    IsActive = companyBranch.IsActive,
                    MaxTicketsPerDay = branchConfiguration.MaxTicketsPerDay,
                    OpenTime = branchConfiguration.OpenTime,
                    CloseTime = branchConfiguration.CloseTime,
                    BreakStartTime = branchConfiguration.BreakStartTime,
                    BreakEndTime = branchConfiguration.BreakEndTime,
                });
            }
        }


        var companyServiceResponseList = new List<CompanyServiceResponseModel>();
        foreach (var service in companyService)
        {
            companyServiceResponseList.Add(new CompanyServiceResponseModel
            {
                Id = service.Id,
                CompanyId = service.CompanyId,
                ServiceName = service.ServiceName,
                ServiceDescription = service.ServiceDescription
            });
        }
        


        var response = new CompanyByIdResponseModel
        {
            Id = dbCompany.Id,
            CompanyName = dbCompany.CompanyName,
            Address = dbCompany.Address,
            EmailAddress = dbCompany.EmailAddress,
            PhoneNumber = dbCompany.PhoneNumber,
            CreatedAt = dbCompany.CreatedAt,
            TotalBranches = branchResponseList.Count,
            Branches = branchResponseList,
            TotalServices = companyServiceResponseList.Count,
            Services = companyServiceResponseList
        };

        _logger.LogInformation("Company with Id {CompanyId} fetched successfully", request.Id);

        return response;
    }
}