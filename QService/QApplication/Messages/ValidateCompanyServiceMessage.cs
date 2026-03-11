namespace QApplication.Messages;

public class ValidateCompanyServiceMessage
{
    public Guid RequestId { get; set; }
    public int CompanyServiceId { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
}