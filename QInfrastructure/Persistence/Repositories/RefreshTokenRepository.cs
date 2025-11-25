using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class RefreshTokenRepository: IRefreshTokenRepository
{
    private readonly QueueDbContext _dbContext;
    private readonly DbSet<RefreshTokenEntity> _dbToken;

    public RefreshTokenRepository(QueueDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbToken = dbContext.Set<RefreshTokenEntity>();
    }

    public RefreshTokenEntity? FindByToken(string token)
    {
        return _dbToken
            .Include(s => s.User)
            .FirstOrDefault(s => s.Token == token);
    }

    public IQueryable<RefreshTokenEntity> GetByUser(int userId)
    {
        return _dbToken.Where(s => s.UserId == userId);
    }

    public void Add(RefreshTokenEntity entity)
    {
        _dbToken.Add(entity);
    }

    public void Update(RefreshTokenEntity entity)
    {
        _dbToken.Update(entity);
    }

    public void Delete(RefreshTokenEntity entity)
    {
        _dbToken.Remove(entity);
    }

    public int SaveChanges()
    {
        return _dbContext.SaveChanges();
    }
}