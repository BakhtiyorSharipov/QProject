namespace QApiGateway.DTO.AuthDTO;

public class ResetPasswordRequest
{
    public string EmailAddress { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}