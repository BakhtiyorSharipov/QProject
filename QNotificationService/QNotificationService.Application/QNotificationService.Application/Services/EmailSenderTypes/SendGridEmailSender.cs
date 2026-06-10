using Microsoft.Extensions.Options;
using QNotificationService.Application.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace QNotificationService.Application.Services.EmailSenderTypes;

public class SendGridEmailSender : IEmailSender
{
    private readonly SendGridSettings _gridSettings;

    public SendGridEmailSender(IOptions<SendGridSettings> gridSettings)
    {
        _gridSettings = gridSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var client = new SendGridClient(_gridSettings.ApiKey);
        var from = new EmailAddress(_gridSettings.SenderEmail, _gridSettings.SenderName);
        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from,
            to,
            subject,
            plainTextContent: body,
            htmlContent: $"<p>{body}</p>"
        );

        var response = await client.SendEmailAsync(msg);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid failed: {error}");
        }
    }
}