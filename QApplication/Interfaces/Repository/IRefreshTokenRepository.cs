using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IRefreshTokenRepository
{
   Task<RefreshTokenEntity?> FindByTokenAsync(string token);
    IQueryable<RefreshTokenEntity> GetByUser(int userId);
    Task AddAsync(RefreshTokenEntity entity);
    void Update(RefreshTokenEntity entity);
    void Delete(RefreshTokenEntity entity);
    Task<int> SaveChangesAsync();
}