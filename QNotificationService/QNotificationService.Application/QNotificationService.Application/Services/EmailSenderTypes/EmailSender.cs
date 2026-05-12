using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using QNotificationService.Application.Interfaces;

namespace QNotificationService.Application.Services.EmailSenderTypes;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;
    

    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        
        email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        
        email.Body = new TextPart("plain")
        {
            Text = body
        };
        
        using var smtp = new SmtpClient();
        
        
        await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port,
            MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
        
        await smtp.SendAsync(email);
        
        await smtp.DisconnectAsync(true);
        
        
    }
}

