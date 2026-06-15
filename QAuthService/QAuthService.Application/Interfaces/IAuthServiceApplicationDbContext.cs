using Microsoft.EntityFrameworkCore;
using QAuthService.Domain.Models;

namespace QAuthService.Application.Interfaces;

public interface IAuthServiceApplicationDbContext
{
    DbSet<UserEntity> Users { get; set; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
}