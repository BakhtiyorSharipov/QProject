using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QDomain.Models;

namespace QApplication.UseCases.Employees.DeleteEmployee;

public class DeleteEmployeeCommandHandler: IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly ILogger<DeleteEmployeeCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public DeleteEmployeeCommandHandler(ILogger<DeleteEmployeeCommandHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting employee with Id {id}", request.Id);

        var dbEmployee = await _dbContext.Employees.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbEmployee== null)
        {
            _logger.LogWarning("Employee with Id {id} not found for deleting", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        _dbContext.Employees.Remove(dbEmployee);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Employee with Id {id} deleted successfully", request.Id);
        return true;
    }
}