namespace QApplication.Responses;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    public int UserId { get; set; }
    public string EmailAddress { get; set; } = null!;
    public string Role { get; set; } = null!;
}