namespace QBranchService.Application.Response;

public class BranchResponseModel
{
    public int Id { get; set; }
    public string BranchName { get; set; } 
    public string City { get; set; }
    public string Address { get; set; } 
    public string PhoneNumber { get; set; } 
    public string EmailAddress { get; set; } 
    public bool IsActive { get; set; } 
    public DateTimeOffset CreatedAt { get; set; } 
    public int CompanyId { get; set; }
}