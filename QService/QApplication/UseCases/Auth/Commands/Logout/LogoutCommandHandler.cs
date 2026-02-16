using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;

namespace QApplication.UseCases.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly ILogger<LogoutCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public LogoutCommandHandler(ILogger<LogoutCommandHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logout with {token} refresh token", request.refreshToken);
        var stored = await _dbContext.RefreshTokens
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Token == request.refreshToken, cancellationToken);
        if (stored == null)
        {
            _logger.LogWarning("Token was not found");
            return;
        }

        stored.RevokedAt = DateTime.UtcNow;


        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Token successfully updated");
    }
}