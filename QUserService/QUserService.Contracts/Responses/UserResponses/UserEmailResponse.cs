using MessagePack;

namespace QUserService.Contracts.Responses.UserResponses;

[MessagePackObject]
public class UserEmailResponse
{
    [Key(0)]
    public int UserId { get; set; }
    
    [Key(1)]
    public string EmailAddress { get; set; } = string.Empty;
    
    [Key(2)]
    public int? CustomerId { get; set; }
    
    [Key(3)]
    public int? EmployeeId { get; set; }
    
    [Key(4)]
    public bool IsValid { get; set; }
    
    [Key(5)]
    public string? ErrorMessage { get; set; }
}