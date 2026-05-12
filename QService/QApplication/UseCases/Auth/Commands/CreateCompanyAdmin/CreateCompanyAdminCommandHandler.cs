using System.Net;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QBranchService.Contracts.Interfaces;
using QBranchService.Contracts.Requests;
using QDomain.Enums;
using QDomain.Models;
using QNotificationService.Contracts.NotificationEvents;

namespace QApplication.UseCases.Auth.Commands.CreateCompanyAdmin;

public class CreateCompanyAdminCommandHandler: IRequestHandler<CreateCompanyAdminCommand, UserEntity>
{
    private readonly ILogger<CreateCompanyAdminCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly IBranchService _branchService;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateCompanyAdminCommandHandler(ILogger<CreateCompanyAdminCommandHandler> logger, 
        IQueueApplicationDbContext dbContext, 
        IPasswordHasher<UserEntity> passwordHasher, IBranchService branchService, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _branchService = branchService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<UserEntity> Handle(CreateCompanyAdminCommand request, CancellationToken cancellationToken)
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
        

        var company = await _branchService.CheckCompanyId(new CompanyRequest
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            RequestedAt = DateTimeOffset.UtcNow
        });

        if (!company.IsValid)
        {
            _logger.LogInformation("Company with Id {companyId} not found", request.CompanyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                company.ErrorMessage ?? "Company not found");
        }

        var employee = new EmployeeEntity
        {
            CompanyId = request.CompanyId,
            BranchId = null,
            ServiceId = null,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            PhoneNumber = request.PhoneNumber,
            Position = request.Position
        };

        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var user = new UserEntity
        {
            EmployeeId = employee.Id,
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.CompanyAdmin
        };

        _logger.LogDebug("Hashing password");
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        var code = new Random().Next(10000,999999).ToString();
        user.EmailVerificationCode = code;
        user.EmailVerificationCodeExpires = DateTime.UtcNow.AddMinutes(10);
        
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        await _publishEndpoint.Publish(new SendNotificationEvent
        {
            Email = user.EmailAddress,
            Message = $@"
                Welcome to Queue System!

                Your verification code is: {code}

                This code will expire in 10 minutes.
                ",
            UserId = user.Id
        }, cancellationToken);
        
        _logger.LogInformation("Employee with {email} email address registered successfully", request.EmailAddress);
        return user;
    }
}