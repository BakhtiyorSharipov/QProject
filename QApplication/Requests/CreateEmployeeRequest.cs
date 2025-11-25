namespace QApplication.Requests;

public class CreateEmployeeRequest
{
    public string EmailAddress { get; set; } = null!; 
    public string Password { get; set; } = null!;
}