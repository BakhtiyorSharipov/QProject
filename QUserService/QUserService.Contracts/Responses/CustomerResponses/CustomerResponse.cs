using MessagePack;

namespace QUserService.Contracts.Responses.CustomerResponses;

[MessagePackObject]
public class CustomerResponse
{
    [Key(0)]
    public int Id { get; set; }
    
    [Key(1)]
    public string FirstName { get; set; } = string.Empty;
    
    [Key(2)]
    public string LastName { get; set; } = string.Empty;
    
    [Key(3)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Key(4)]
    public DateTime CreatedAt { get; set; }
    
    [Key(5)]
    public DateTime? DateOfBirth { get; set; }
    
    [Key(6)]
    public string? Gender { get; set; }
    
    [Key(7)]
    public string? Country { get; set; }
    
    [Key(8)]
    public string? City { get; set; }
    
    [Key(9)]
    public string? Address { get; set; }
    
    [Key(10)]
    public string? PostalCode { get; set; }
    
    [Key(11)]
    public bool IsValid { get; set; }
    
    [Key(12)]
    public string? ErrorMessage { get; set; }
}





