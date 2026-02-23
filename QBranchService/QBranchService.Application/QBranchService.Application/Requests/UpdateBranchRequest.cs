namespace QBranchService.Application.Requests;

public class UpdateBranchRequest
{
    public int Id { get; set; }
    public string BranchName { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
}