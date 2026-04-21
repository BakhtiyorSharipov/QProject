namespace QBranchService.Application.Response;

public class CompanyBranchesResponseModel
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string BranchName { get; set; } 
    public string City { get; set; }
    public string Address { get; set; } 
    public string PhoneNumber { get; set; } 
    public string EmailAddress { get; set; } 
    public bool IsActive { get; set; } 
    public int MaxTicketsPerDay { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public TimeOnly? BreakStartTime { get; set; }
    public TimeOnly? BreakEndTime { get; set; }
}