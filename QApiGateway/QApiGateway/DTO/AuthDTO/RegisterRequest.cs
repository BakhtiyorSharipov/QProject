namespace QApiGateway.DTO.AuthDTO;

public class RegisterRequest
{
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
}