using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using QUserService.Application.Exceptions;
using QUserService.Application.Extensions;
using QUserService.Application.Interfaces;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Auth.Commands.UpdateUserPassword;

public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand, bool>
{
    private readonly ILogger<UpdateUserPasswordCommandHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;

    public UpdateUserPasswordCommandHandler(ILogger<UpdateUserPasswordCommandHandler> logger,
        IUserServiceApplicationDbContext dbContext, IHttpContextAccessor contextAccessor,
        IPasswordHasher<UserEntity> passwordHasher)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user password");
        var currentUser = await _contextAccessor.CurrentUser(_dbContext, cancellationToken);

        var currentPassword =
            _passwordHasher.VerifyHashedPassword(currentUser, currentUser.PasswordHash, request.OldPassword);
        if (currentPassword == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Current Password is incorrect");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Current Password is incorrect");
        }

        currentUser.PasswordHash = _passwordHasher.HashPassword(currentUser, request.NewPassword);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}