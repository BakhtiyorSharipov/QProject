namespace QApiGateway.DTO.AuthDTO;

public class VerifyEmailRequest
{
    public string EmailAddress { get; set; }
    public string Code { get; set; }
}