using MessagePack;

namespace QUserService.Contracts.Responses.UserResponses;

[MessagePackObject]
public class CurrentCustomerResponse
{
    [Key(0)]
    public int CustomerId { get; set; }
    
    [Key(1)]
    public string FirstName { get; set; } = string.Empty;
    
    [Key(2)]
    public string LastName { get; set; } = string.Empty;
    
    [Key(3)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Key(4)]
    public bool IsValid { get; set; }
    
    [Key(5)]
    public string? ErrorMessage { get; set; }
}