using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class QueueRepository:  IQueueRepository
{
    private readonly DbSet<QueueEntity> _dbQueue;
    private readonly EFContext _context;

    public QueueRepository(EFContext context)
    {
        _dbQueue = context.Set<QueueEntity>();
        _context = context;
    }

    public IQueryable<QueueEntity> GetAll(int pageList, int pageNumber)
    {
        return _dbQueue.Skip((pageNumber - 1) * pageList).Take(pageList);
    }

    public QueueEntity FindById(int id)
    {
        var found = _dbQueue.Find(id);
        return found;
    }

    public void Add(QueueEntity entity)
    {
        _dbQueue.Add(entity);
    }

    public void Update(QueueEntity entity)
    {
        _dbQueue.Update(entity);
    }

    public void Delete(QueueEntity entity)
    {
        _dbQueue.Remove(entity);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}