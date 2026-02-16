using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Services.Queries.GetServiceById;

public class GetServiceByIdQueryHandler: IRequestHandler<GetServiceByIdQuery, ServiceResponseModel>
{
    private readonly ILogger<GetServiceByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetServiceByIdQueryHandler(ILogger<GetServiceByIdQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ServiceResponseModel> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting service by Id {id}", request.Id);
        var dbService = await _dbContext.Services.FirstOrDefaultAsync(s=>s.Id== request.Id, cancellationToken);
        if (dbService == null)
        {
            _logger.LogWarning("Service with Id {id} not found.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        var response = new ServiceResponseModel()
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