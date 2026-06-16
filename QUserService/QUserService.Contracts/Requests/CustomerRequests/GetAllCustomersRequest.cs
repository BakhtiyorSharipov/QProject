using MessagePack;

namespace QUserService.Contracts.Requests.CustomerRequests;

[MessagePackObject]
public class GetAllCustomersRequest
{
    [Key(0)]
    public int PageNumber { get; set; } = 1;
    
    [Key(1)]
    public int PageSize { get; set; } = 15;
    
    [Key(2)]
    public int? CompanyId { get; set; }
    
    [Key(3)]
    public Guid RequestId { get; set; }
}