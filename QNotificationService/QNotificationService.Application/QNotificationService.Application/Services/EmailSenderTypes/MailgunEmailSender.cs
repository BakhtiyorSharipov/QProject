using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using QNotificationService.Application.Interfaces;

namespace QNotificationService.Application.Services.EmailSenderTypes;

public class MailgunEmailSender: IEmailSender
{
    private readonly MailgunSettings _settings;
    private readonly HttpClient _httpClient;

    public MailgunEmailSender(IOptions<MailgunSettings> settings)
    {
        _settings = settings.Value;
        _httpClient = new HttpClient();

        var byteArray = Encoding.ASCII.GetBytes($"api:{_settings.ApiKey}");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
    }
    
    
    
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api.mailgun.net/v3/{_settings.Domain}/messages"
        );

        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("from", _settings.FromEmail),
            new KeyValuePair<string, string>("to", toEmail),
            new KeyValuePair<string, string>("subject", subject),
            new KeyValuePair<string, string>("text", body)
        });

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Mailgun failed: {error}");
        }
    }
}