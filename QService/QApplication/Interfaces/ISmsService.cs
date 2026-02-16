namespace QApplication.Interfaces;

public interface ISmsService
{
    Task Send(int customerId, string message, CancellationToken cancellationToken);
}