namespace QNotificationService.Application;

public class MailgunSettings
{
    public string ApiKey { get; set; } = null!;
    public string Domain { get; set; } = null!;
    public string FromEmail { get; set; } = null!;
}