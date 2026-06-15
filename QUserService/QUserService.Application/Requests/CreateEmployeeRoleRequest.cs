namespace QUserService.Application.Requests;

public class CreateEmployeeRoleRequest
{
    public int BranchId { get; set; }
    public int ServiceId { get; set; }
    public string EmailAddress { get; set; } = null!; 
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Position { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}