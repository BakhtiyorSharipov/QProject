using System.Net;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Extensions;
using QApplication.Interfaces.Data;
using QApplication.Messages;
using QBranchService.Contracts.Requests;
using QBranchService.Contracts.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.Auth.Commands.CreateEmployee;

public class CreateEmployeeRoleCommandHandler : IRequestHandler<CreateEmployeeRoleCommand, UserEntity>
{
    private readonly ILogger<CreateEmployeeRoleCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly IRequestClient<BranchIdsRequest> _validationClient;
    private readonly IHttpContextAccessor _contextAccessor;

    public CreateEmployeeRoleCommandHandler(ILogger<CreateEmployeeRoleCommandHandler> logger,
        IQueueApplicationDbContext dbContext, IPasswordHasher<UserEntity> passwordHasher,
        IRequestClient<BranchIdsRequest> validationClient, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _validationClient = validationClient;
        _contextAccessor = contextAccessor;
    }

    public async Task<UserEntity> Handle(CreateEmployeeRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering employee with {email} email address", request.EmailAddress);
        _logger.LogDebug("Finding creator Id for registering employee");
        var creator =
            await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == request.createdByUserId, cancellationToken);
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
        if (await _dbContext.Users.FirstOrDefaultAsync(s => s.EmailAddress == request.EmailAddress,
                cancellationToken) != null)
        {
            _logger.LogWarning("Email is already exists.");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Email already exists");
        }

        if (!request.ServiceId.HasValue)
        {
            _logger.LogError("ServiceId is required for creating an employee");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                "ServiceId is required for creating an employee");
        }

        var currentEmployee = await _contextAccessor.CurrentEmployee(_dbContext,cancellationToken);

        var companyId = currentEmployee.CompanyId;

        var validationResponse = await _validationClient.GetResponse<BranchIdsResponse>(
            new ValidateBranchIdsMessage()
            {
                RequestId = Guid.NewGuid(),
                CompanyId = companyId,
                BranchId = request.BranchId,
                CompanyServiceId = request.ServiceId.Value,
                RequestedAt = DateTimeOffset.UtcNow
            }, cancellationToken, RequestTimeout.After(s: 15));

        if (!validationResponse.Message.IsValid)
        {
            _logger.LogWarning("Validation failed: {ErrorMessage}", validationResponse.Message.ErrorMessage);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                validationResponse.Message.ErrorMessage ?? "Invalid companyId or BranchId or CompanyServiceId");
        }
        
        

        var employee = new EmployeeEntity
        {
            CompanyId = companyId,
            BranchId = request.BranchId,
            ServiceId = request.ServiceId.Value,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Position = request.Position,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var user = new UserEntity
        {
            EmployeeId = employee.Id,
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.Employee
        };


        _logger.LogDebug("Hashing password");
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Employee with {email} email address registered successfully", request.EmailAddress);
        return user;
    }
}