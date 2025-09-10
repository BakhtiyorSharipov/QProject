namespace QApplication.Requests.BlockedCustomerRequest;

public class BlockedCustomerRequestModel: BaseRequest
{
    public int CompanyId { get; set; }
    public int CustomerId { get; set; }
    public string? Reason { get; set; }
    public DateTime BannedUntil { get; set; }
    public bool DoesBanForever { get; set; }
}