using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.CompanyServices.Queries.GetServiceById;

public class GetServiceByIdQueryHandler: IRequestHandler<GetServiceByIdQuery, CompanyServiceResponseModel>
{
    private readonly ILogger<GetServiceByIdQueryHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public GetServiceByIdQueryHandler(ILogger<GetServiceByIdQueryHandler> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<CompanyServiceResponseModel> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting service by Id {id}", request.Id);
        var dbService = await _dbContext.CompanyServices.FirstOrDefaultAsync(s=>s.Id== request.Id, cancellationToken);
        if (dbService == null)
        {
            _logger.LogWarning("Service with Id {id} not found.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyServiceEntity));
        }

        var response = new CompanyServiceResponseModel()
        {
            Id = dbService.Id,
            CompanyId = dbService.CompanyId,
            ServiceName = dbService.ServiceName,
            ServiceDescription = dbService.ServiceDescription
        };

        _logger.LogInformation("Service with Id {id} fetched successfully.", request.Id);
        return response;
    }
}