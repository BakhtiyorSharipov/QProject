namespace QDomain.Models;

public class RefreshTokenEntity: BaseEntity
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}