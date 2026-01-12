using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Requests.ServiceRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Services.Commands.UpdateService;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, ServiceResponseModel>
{
    private readonly ILogger<UpdateServiceCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public UpdateServiceCommandHandler(ILogger<UpdateServiceCommandHandler> logger,
        IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ServiceResponseModel> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating service with Id {id}.", request.Id);

        var dbService = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbService == null)
        {
            _logger.LogWarning("Service with Id {id} not found for updating.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }


        dbService.CompanyId = request.CompanyId;
        dbService.ServiceName = request.ServiceName;
        dbService.ServiceDescription = request.ServiceDescription;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Service with Id {id} updated successfully.", request.Id);

        var response = new ServiceResponseModel()
        {
            Id = dbService.Id,
            CompanyId = dbService.CompanyId,
            ServiceName = dbService.ServiceName,
            ServiceDescription = dbService.ServiceDescription
        };

        return response;
    }
}