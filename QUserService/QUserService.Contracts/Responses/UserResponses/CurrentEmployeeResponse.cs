using MessagePack;

namespace QUserService.Contracts.Responses.UserResponses;

[MessagePackObject]
public class CurrentEmployeeResponse
{
    [Key(0)]
    public int EmployeeId { get; set; }
    
    [Key(1)]
    public int CompanyId { get; set; }
    
    [Key(2)]
    public int? BranchId { get; set; }
    
    [Key(3)]
    public string FirstName { get; set; } = string.Empty;
    
    [Key(4)]
    public string LastName { get; set; } = string.Empty;
    
    [Key(5)]
    public string Position { get; set; } = string.Empty;
    
    [Key(6)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Key(7)]
    public bool IsValid { get; set; }
    
    [Key(8)]
    public string? ErrorMessage { get; set; }
}