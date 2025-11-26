using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class UserRepository: IUserRepository
{
    private readonly QueueDbContext _dbContext;
    private readonly DbSet<User> _dbUser;

    public UserRepository(QueueDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbUser = dbContext.Set<User>();
    }
    
    public async Task<User?> FindByIdAsync(int id)
    {
        return await _dbUser.FindAsync(id);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _dbUser.FirstOrDefaultAsync(s => s.EmailAddress == email);
    }

    public async Task AddAsync(User entity)
    {
        await _dbUser.AddAsync(entity);
    }

    public void Update(User entity)
    {
        _dbUser.Update(entity);
    }

    public void Delete(User entity)
    {
        _dbUser.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
       return await _dbContext.SaveChangesAsync();
    }
}