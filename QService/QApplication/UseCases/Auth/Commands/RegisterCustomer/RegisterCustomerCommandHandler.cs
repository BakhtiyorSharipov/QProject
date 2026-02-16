using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QDomain.Enums;
using QDomain.Models;


namespace QApplication.UseCases.Auth.Commands.RegisterCustomer;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, User>
{
    private readonly ILogger<RegisterCustomerCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterCustomerCommandHandler(ILogger<RegisterCustomerCommandHandler> logger,
        IQueueApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _logger = logger;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering customer with {email} email address", request.EmailAddress);
        if (await _dbContext.Users.FirstOrDefaultAsync(s => s.EmailAddress == request.EmailAddress, cancellationToken) != null)
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

        await _dbContext.Customers.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);


        var user = new User
        {
            CustomerId = customer.Id,
            EmailAddress = request.EmailAddress,
            Roles = UserRoles.Customer,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogDebug("Hashing password");
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Customer registered successfully");
        return user;
    }
}