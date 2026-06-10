namespace QApiGateway.DTO.AuthDTO;

public class UpdatePasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}