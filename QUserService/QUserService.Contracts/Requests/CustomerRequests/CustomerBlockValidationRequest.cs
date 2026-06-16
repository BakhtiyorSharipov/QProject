using MessagePack;

namespace QUserService.Contracts.Requests.CustomerRequests;

[MessagePackObject]
public class CustomerBlockValidationRequest
{
    [Key(0)]
    public int CustomerId { get; set; }
    
    [Key(1)]
    public int CompanyId { get; set; }
    
    [Key(2)]
    public Guid RequestId { get; set; }
}