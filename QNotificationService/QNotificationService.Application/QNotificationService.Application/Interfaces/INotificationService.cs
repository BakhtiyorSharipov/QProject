namespace QNotificationService.Application.Interfaces;

public interface INotificationService
{
    Task SendAsync(int customerId, string message, CancellationToken cancellationToken);
}