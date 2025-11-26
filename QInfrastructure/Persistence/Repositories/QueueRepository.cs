using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class QueueRepository:  IQueueRepository
{
    private readonly DbSet<QueueEntity> _dbQueue;
    private readonly QueueDbContext _context;

    public QueueRepository(QueueDbContext context)
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
        return _dbQueue
            .Include(q => q.Service)
            .ThenInclude(s => s.Company)
            .Include(q => q.Customer)
            .Include(q => q.Employee)
            .FirstOrDefault(q => q.Id == id);
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
    
    public IQueryable<QueueEntity> GetQueuesByCustomer(int customerId)
    {
        var found = _dbQueue.Where(q=>q.CustomerId==customerId);
        return found;
    }

    public IQueryable<QueueEntity> GetQueuesByEmployee(int employeeId)
    {
        var found = _dbQueue.Where(q => q.EmployeeId == employeeId);
        return found;
    }
}