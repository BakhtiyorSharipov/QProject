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

    public async Task<RefreshTokenEntity?> FindByTokenAsync(string token)
    {
        return await _dbToken
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Token == token);
    }

    public IQueryable<RefreshTokenEntity> GetByUser(int userId)
    {
        return _dbToken.Where(s => s.UserId == userId);
    }

    public async Task AddAsync(RefreshTokenEntity entity)
    {
         await  _dbToken.AddAsync(entity);
    }

    public void Update(RefreshTokenEntity entity)
    {
        _dbToken.Update(entity);
    }

    public void Delete(RefreshTokenEntity entity)
    {
        _dbToken.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}