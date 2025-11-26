namespace QApplication.Requests.EmployeeRequest;

public class EmployeeRequestModel: BaseRequest
{
    public int ServiceId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Position { get; set; }
    public string PhoneNumber { get; set; }
}