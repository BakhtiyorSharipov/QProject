using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QBranchService.Application.Exceptions;
using QBranchService.Application.Interfaces.Data;
using QBranchService.Domain.Models;

namespace QBranchService.Application.UseCases.CompanyServices.Commands.DeleteService;

public class DeleteServiceCommandHandler: IRequestHandler<DeleteServiceCommand, bool>
{
    private readonly ILogger<DeleteServiceCommandHandler> _logger;
    private readonly IBranchServiceApplicationDbContext _dbContext;

    public DeleteServiceCommandHandler(ILogger<DeleteServiceCommandHandler> logger, IBranchServiceApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting service with Id {id}", request.Id);
        var dbService = await _dbContext.CompanyServices.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbService == null)
        {
            _logger.LogWarning("Service with Id {id} not found for deleting.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyServiceEntity));
        }

        _dbContext.CompanyServices.Remove(dbService);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Service with Id {id} deleted successfully.", request.Id);

        return true;
    }
}