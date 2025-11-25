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

    public IQueryable<User> Queryable()
    {
        return _dbUser;
    }

    public User? FindById(int id)
    {
        return _dbUser.Find(id);
    }

    public User? FindByEmail(string email)
    {
        return _dbUser.FirstOrDefault(s => s.EmailAddress == email);
    }

    public void Add(User entity)
    {
        _dbUser.Add(entity);
    }

    public void Update(User entity)
    {
        _dbUser.Update(entity);
    }

    public void Delete(User entity)
    {
        _dbUser.Remove(entity);
    }

    public int SaveChanges()
    {
       return  _dbContext.SaveChanges();
    }
}