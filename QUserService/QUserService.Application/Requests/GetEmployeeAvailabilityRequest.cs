namespace QUserService.Application.Requests;

public class GetEmployeeAvailabilityRequest
{
    public int EmployeeId { get; set; }

    public DateTimeOffset Date { get; set; }
    
}