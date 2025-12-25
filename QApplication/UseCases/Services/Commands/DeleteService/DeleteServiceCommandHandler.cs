using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QDomain.Models;

namespace QApplication.UseCases.Services.Commands.DeleteService;

public class DeleteServiceCommandHandler: IRequestHandler<DeleteServiceCommand, bool>
{
    private readonly ILogger<DeleteServiceCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public DeleteServiceCommandHandler(ILogger<DeleteServiceCommandHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting service with Id {id}", request.Id);
        var dbService = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbService == null)
        {
            _logger.LogWarning("Service with Id {id} not found for deleting.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
        }

        _dbContext.Services.Remove(dbService);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Service with Id {id} deleted successfully.", request.Id);

        return true;
    }
}