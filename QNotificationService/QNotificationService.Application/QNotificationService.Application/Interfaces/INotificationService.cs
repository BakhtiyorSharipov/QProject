namespace QNotificationService.Application.Interfaces;

public interface INotificationService
{
    Task SendAsync(string email,  int customerId,  string message, CancellationToken cancellationToken);
}