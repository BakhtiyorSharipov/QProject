namespace QApplication.Requests;

public class RegisterCustomerRequestModel
{
    public string EmailAddress { get; set; } = null!; 
    public string Password { get; set; } = null!;
}