using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class Repository<TEntity>: IRepository<TEntity> where TEntity: BaseEntity
{
    private readonly DbSet<TEntity> _set;
    private readonly EFContext _context;

    public Repository(EFContext context)
    {
        _set = context.Set<TEntity>();
        _context = context;
    }
    
    public IQueryable<TEntity> GetAll(int pageList, int pageNumber)
    {
        return _set.Skip((pageNumber - 1) * pageList).Take(pageList);
    }

    public TEntity FindById(int id)
    {
        var foundEntity = _set.Find(id);
        return foundEntity;
    }

    public void Add(TEntity entity)
    {
        _set.Add(entity);
    }

    public void Update(TEntity entity)
    {
        _set.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _set.Remove(entity);
    }

    public int SaveChanges()
    {
       return _context.SaveChanges();
    }
}