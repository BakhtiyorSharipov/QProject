using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Services.Commands.CreateService;

public class CreateServiceCommandHandler: IRequestHandler<CreateServiceCommand, ServiceResponseModel>
{
    private readonly ILogger<CreateServiceCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public CreateServiceCommandHandler(ILogger<CreateServiceCommandHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ServiceResponseModel> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding new service with Name {request.ServiceName}", request.ServiceName);
        

        var service = new ServiceEntity()
        {
            CompanyId = request.CompanyId,
            ServiceName = request.ServiceName,
            ServiceDescription = request.ServiceDescription,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Services.AddAsync(service, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Service {service.ServiceName} added successfully with Id {service.Id}.",
            service.ServiceName);

        var response = new ServiceResponseModel()
        {
            Id = service.Id,
            CompanyId = service.CompanyId,
            ServiceName = service.ServiceName,
            ServiceDescription = service.ServiceDescription
        };

        return response;
    }
}