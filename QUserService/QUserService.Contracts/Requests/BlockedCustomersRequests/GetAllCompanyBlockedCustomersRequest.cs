using MessagePack;

namespace QUserService.Contracts.Requests.BlockedCustomersRequests;

[MessagePackObject]
public class GetAllCompanyBlockedCustomersRequest
{
    [Key(0)]
    public int CompanyId { get; set; }
    
    [Key(1)]
    public int PageNumber { get; set; } = 1;
    
    [Key(2)]
    public int PageSize { get; set; } = 15;
    
    [Key(3)]
    public Guid RequestId { get; set; }
}
