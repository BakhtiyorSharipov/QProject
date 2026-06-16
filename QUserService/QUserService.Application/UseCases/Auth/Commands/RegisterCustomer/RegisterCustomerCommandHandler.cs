using System.Net;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QAuthService.Contracts.Events.CustomerEvent;
using QNotificationService.Contracts.NotificationEvents;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;
using QUserService.Domain.Enums;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Auth.Commands.RegisterCustomer;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, UserEntity>
{
    private readonly ILogger<RegisterCustomerCommandHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly IPublishEndpoint _publishEndpoint;

    public RegisterCustomerCommandHandler(ILogger<RegisterCustomerCommandHandler> logger,
        IUserServiceApplicationDbContext dbContext, IPasswordHasher<UserEntity> passwordHasher,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<UserEntity> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering customer with {email} email address", request.EmailAddress);
        if (await _dbContext.Users.FirstOrDefaultAsync(s => s.EmailAddress == request.EmailAddress,
                cancellationToken) != null)
        {
            _logger.LogWarning("Customer with {email} email address already exists", request.EmailAddress);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Email address already exists");
        }

        var customer = new CustomerEntity()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Customer.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new CustomerCreatedEvent
        {
            OccuredAt = DateTimeOffset.UtcNow,
            CustomerId = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber
        }, cancellationToken);


        var user = new UserEntity
        {
            CustomerId = customer.Id,
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.Customer,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogDebug("Hashing password");
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        var code = GenerateVerificationCode();
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
        _logger.LogInformation("Customer registered successfully");
        return user;
    }

    private string GenerateVerificationCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}