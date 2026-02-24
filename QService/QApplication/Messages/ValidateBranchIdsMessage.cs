namespace QApplication.Messages;

public class ValidateBranchIdsMessage
{
    public Guid RequestId { get; set; }
    public int CompanyId { get; set; }
    public int BranchId { get; set; }
    public int CompanyServiceId { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
}