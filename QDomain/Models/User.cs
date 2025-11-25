using QDomain.Enums;

namespace QDomain.Models;

public class User: BaseEntity
{
   
    public string EmailAddress { get; set; }
    public string PasswordHash { get; set; }
    public UserRoles Roles { get; set; }
    public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

    public List<RefreshTokenEntity> RefreshTokens { get; set; } = new();
}