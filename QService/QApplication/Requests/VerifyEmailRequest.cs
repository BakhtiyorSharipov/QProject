namespace QApplication.Requests;

public class VerifyEmailRequest
{
    public string EmailAddress { get; set; }
    public string Code { get; set; }
}