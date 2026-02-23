using MediatR;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Application.Response;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.CompanyServices.Commands.CreateService;

public class CreateServiceCommandHandler: IRequestHandler<CreateServiceCommand, CompanyServiceResponseModel>
{
    private readonly ILogger<CreateServiceCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public CreateServiceCommandHandler(ILogger<CreateServiceCommandHandler> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<CompanyServiceResponseModel> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding new service with Name {request.ServiceName}", request.ServiceName);
        

        var service = new CompanyServiceEntity
        {
            CompanyId = request.CompanyId,
            ServiceName = request.ServiceName,
            ServiceDescription = request.ServiceDescription,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _dbContext.CompanyServices.AddAsync(service, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Service {service.ServiceName} added successfully with Id {service.Id}.",
            service.ServiceName, service.Id);

        var response = new CompanyServiceResponseModel()
        {
            Id = service.Id,
            CompanyId = service.CompanyId,
            ServiceName = service.ServiceName,
            ServiceDescription = service.ServiceDescription
        };

        return response;
    }
}