using Microsoft.EntityFrameworkCore;
using QUserService.Domain.Models;

namespace QUserService.Application.Interfaces;

public interface IUserServiceApplicationDbContext
{
    DbSet<UserEntity> Users { get; set; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
}