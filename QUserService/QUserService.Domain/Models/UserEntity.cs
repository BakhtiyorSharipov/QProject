using QUserService.Domain.Enums;

namespace QUserService.Domain.Models;

public class UserEntity
{
    public int Id { get; set; }
    public string EmailAddress { get; set; }
    public string PasswordHash { get; set; }
    public UserRoles Roles { get; set; }
    public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

    public bool IsEmailVerified { get; set; } = false;
    public string? EmailVerificationCode { get; set; }
    public DateTime? EmailVerificationCodeExpires { get; set; }
    public DateTime? VerifiedAt { get; set; }
    
    public int ResendCount { get; set; } = 0;
    public DateTime? LastCodeSentAt { get; set; }
    
    public string? PasswordResetCode { get; set; }
    public DateTime? PasswordResetExpiry { get; set; }
    
    public List<RefreshTokenEntity> RefreshTokens { get; set; } = new();

    public int? EmployeeId { get; set; }
    public EmployeeEntity? Employee { get; set; }
    
    public int? CustomerId { get; set; }
    public CustomerEntity? Customer { get; set; }
}