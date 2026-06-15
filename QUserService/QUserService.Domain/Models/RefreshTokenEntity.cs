namespace QUserService.Domain.Models;

public class RefreshTokenEntity
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    
    public int UserId { get; set; }
    public UserEntity UserEntity { get; set; } = null!;
}