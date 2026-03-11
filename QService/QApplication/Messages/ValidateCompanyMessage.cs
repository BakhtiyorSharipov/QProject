namespace QApplication.Messages;

public class ValidateCompanyMessage
{
    public Guid RequestId { get; set; }
    public int CompanyId { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
}