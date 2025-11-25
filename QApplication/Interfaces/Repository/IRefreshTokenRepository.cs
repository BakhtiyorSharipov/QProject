using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IRefreshTokenRepository
{
    RefreshTokenEntity? FindByToken(string token);
    IQueryable<RefreshTokenEntity> GetByUser(int userId);
    void Add(RefreshTokenEntity entity);
    void Update(RefreshTokenEntity entity);
    void Delete(RefreshTokenEntity entity);
    int SaveChanges();
}