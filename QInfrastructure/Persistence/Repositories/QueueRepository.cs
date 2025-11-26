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
    

    public async Task<QueueEntity> FindByIdAsync(int id)
    {
        var found= await _dbQueue
            .Include(q => q.Service)
            .ThenInclude(s => s.Company)
            .Include(q => q.Customer)
            .Include(q => q.Employee)
            .FirstOrDefaultAsync(q => q.Id == id);
        return found;
    }


    public async Task AddAsync(QueueEntity entity)
    {
        await _dbQueue.AddAsync(entity);
    }

    public void Update(QueueEntity entity)
    {
        _dbQueue.Update(entity);
    }

    public void Delete(QueueEntity entity)
    {
        _dbQueue.Remove(entity);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
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

    public IQueryable<QueueEntity> GetQueuesByService(int serviceId)
    {
        var found = _dbQueue.Where(q => q.Service.Id == serviceId);
        return found;
    }

    public IQueryable<QueueEntity> GetQueuesByCompany(int companyId)
    {
        var found = _dbQueue.Where(q => q.Service.CompanyId == companyId);
        return found;
    }

    public IQueryable<QueueEntity> GetAllQueues()
    {
        var queues = _dbQueue
            .Include(q => q.Employee)
            .Include(q => q.Customer)
            .Include(q => q.Service)
            .Include(q => q.Service.Company)
            .Include(s => s.Customer.Reviews)
            .Include(s => s.Customer.Complaints);
        return queues;
    }
}