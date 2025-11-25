namespace QApplication.Requests;

public class CreateCompanyAdminRequest
{
    public string EmailAddress { get; set; } = null!; 
    public string Password { get; set; } = null!;
}