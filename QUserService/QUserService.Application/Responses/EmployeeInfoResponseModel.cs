namespace QUserService.Application.Responses;

public class EmployeeInfoResponseModel
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int? BranchId { get; set; }
    public int ServiceId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Position { get; set; }
    public string PhoneNumber { get; set; }
    // public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    // public int TotalComplaints { get; set; }
}