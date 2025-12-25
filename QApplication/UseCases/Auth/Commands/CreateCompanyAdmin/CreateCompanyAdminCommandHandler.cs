using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.Auth.Commands.CreateCompanyAdmin;

public class CreateCompanyAdminCommandHandler: IRequestHandler<CreateCompanyAdminCommand, User>
{
    private readonly ILogger<CreateCompanyAdminCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public CreateCompanyAdminCommandHandler(ILogger<CreateCompanyAdminCommandHandler> logger, IQueueApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _logger = logger;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(CreateCompanyAdminCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering companyAdmin with {email} email address", request.EmailAddress);
        _logger.LogDebug("Finding creator Id for registering employee");
        var creator =
            await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == request.createdByUserId, cancellationToken);
        if (creator == null)
        {
            _logger.LogWarning("Creator with Id {id} not found", request.createdByUserId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Creator not found");
        }

        _logger.LogDebug("Checking is creator role valid for creating employee");
        if (creator.Roles != UserRoles.SystemAdmin)
        {
            _logger.LogWarning("Not allowed to create. Creator's role: {role}", creator.Roles);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Not allowed to create barbershop admin");
        }

        _logger.LogDebug("Checking email for already exists emails");
        if (await _dbContext.Users.FirstOrDefaultAsync(s=>s.EmailAddress== request.EmailAddress, cancellationToken) != null)
        {
            
            _logger.LogWarning("Email is already exists.");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Email already exists");
        }

        var employee = new EmployeeEntity
        {
            ServiceId = null,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            PhoneNumber = request.PhoneNumber,
            Position = request.Position
        };

        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var user = new User
        {
            EmployeeId = employee.Id,
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.CompanyAdmin
        };

        _logger.LogDebug("Hashing password");
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Employee with {email} email address registered successfully");
        return user;
    }
}