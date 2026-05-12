using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QNotificationService.Contracts.NotificationEvents;

namespace QApplication.UseCases.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler: IRequestHandler<ForgotPasswordCommand, bool>
{
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public ForgotPasswordCommandHandler(ILogger<ForgotPasswordCommandHandler> logger, IQueueApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(s => s.EmailAddress == request.EmailAddress, cancellationToken);
        if (user ==null)
        {
            _logger.LogError("User with Email {EmailAddress} not found", request.EmailAddress);
            return true;
        }

        var code = new Random().Next(100000, 999999).ToString();

        user.PasswordResetCode = code;
        user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(10);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new SendNotificationEvent
        {
            Email = user.EmailAddress,
            Message = $@"
                Welcome to Queue System!

                Your reset code is: {code}

                This code will expire in 10 minutes.
                ",
            UserId = user.Id
        }, cancellationToken);

        return true;

    }
}