namespace QApplication.Requests.EmployeeRequest;

public class UpdateEmployeeRequest
{
    public int ServiceId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Position { get; set; }
    public string PhoneNumber { get; set; }
}