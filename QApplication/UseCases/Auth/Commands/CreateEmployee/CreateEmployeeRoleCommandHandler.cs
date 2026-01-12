using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.Auth.Commands.CreateEmployee;

public class CreateEmployeeRoleCommandHandler: IRequestHandler<CreateEmployeeRoleCommand, User>
{
    private readonly ILogger<CreateEmployeeRoleCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public CreateEmployeeRoleCommandHandler(ILogger<CreateEmployeeRoleCommandHandler> logger, IQueueApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _logger = logger;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(CreateEmployeeRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering employee with {email} email address", request.EmailAddress);
        _logger.LogDebug("Finding creator Id for registering employee");
        var creator = await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == request.createdByUserId, cancellationToken);
        if (creator == null)
        {
            _logger.LogWarning("Creator with Id {id} not found", request.createdByUserId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Creator not found");
        }

        _logger.LogDebug("Checking is creator role valid for creating employee");
        if (creator.Roles != UserRoles.CompanyAdmin && creator.Roles != UserRoles.SystemAdmin)
        {
            _logger.LogWarning("Not allowed to create. Creator's role: {role}", creator.Roles);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Not allowed to create employee");
        }

        _logger.LogDebug("Checking email for already exists emails");
        if (await _dbContext.Users.FirstOrDefaultAsync(s=>s.EmailAddress== request.EmailAddress, cancellationToken) != null)
        {
            
            _logger.LogWarning("Email is already exists.");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Email already exists");
        }
        
        if (!request.ServiceId.HasValue)
        {
            _logger.LogError("ServiceId is required for creating an employee");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "ServiceId is required for creating an employee");
        }

        var serviceExists = await _dbContext.Services
            .AnyAsync(s => s.Id == request.ServiceId.Value, cancellationToken);
    
        if (!serviceExists)
        {
            _logger.LogError("Service with Id {serviceId} does not exist", request.ServiceId);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, $"Service with ID {request.ServiceId} does not exist");
        }
        
        var employee = new EmployeeEntity
        {
            ServiceId = request.ServiceId.Value,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Position = request.Position,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var user = new User
        {
            EmployeeId = employee.Id,
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.Employee
        };
        
        
        _logger.LogDebug("Hashing password");
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Employee with {email} email address registered successfully");
        return user;
    }
}