using System.Net;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QNotificationService.Contracts.NotificationEvents;
using QUserService.Application.Exceptions;
using QUserService.Application.Interfaces;

namespace QUserService.Application.UseCases.Auth.Commands.ResendCode;

public class ResendVerificationCommandHandler: IRequestHandler<ResendVerificationCodeCommand, bool>
{
    private readonly ILogger<ResendVerificationCommandHandler> _logger;
    private readonly IUserServiceApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public ResendVerificationCommandHandler(ILogger<ResendVerificationCommandHandler> logger, IUserServiceApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(ResendVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.EmailAddress == request.EmailAddress,
            cancellationToken);

        if (user== null)
        {
            _logger.LogWarning("User with Email {EmailAddress} not found", request.EmailAddress);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"User with Email {request.EmailAddress} not found");
        }

        if (user.IsEmailVerified)
        {
            _logger.LogWarning("Email already verified");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Email already verified");
        }

        if (user.LastCodeSentAt.HasValue && (DateTime.UtcNow- user.LastCodeSentAt.Value).TotalSeconds <60)
        {
            _logger.LogWarning("Please request after 1 minute");
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Please request after 1 minute");
        }
        
        if (user.ResendCount >= 5 &&
            user.LastCodeSentAt.HasValue &&
            (DateTime.UtcNow - user.LastCodeSentAt.Value).TotalHours < 1)
        {
            throw new Exception("Too many attempts. Try later.");
        }

        var code = new Random().Next(100000, 999999).ToString();
        user.EmailVerificationCode = code;
        user.EmailVerificationCodeExpires = DateTime.UtcNow.AddMinutes(10);
        user.LastCodeSentAt = DateTime.UtcNow;
        user.ResendCount++;

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new SendNotificationEvent
        {
            Email = user.EmailAddress,
            Message = $@"
                Welcome to Queue System!

                Resend Verification Code

                Your new verification code is: {code}

                This code will expire in 10 minutes.
                ",
            UserId = user.Id
        });

        return true;

    }
}