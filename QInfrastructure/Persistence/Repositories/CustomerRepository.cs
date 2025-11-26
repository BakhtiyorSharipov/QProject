using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class CustomerRepository:  ICustomerRepository
{
    private readonly DbSet<CustomerEntity> _dbCustomer;
    private readonly QueueDbContext _context;

    public CustomerRepository(QueueDbContext context)
    {
        _dbCustomer = context.Set<CustomerEntity>();
        _context = context;
    }

    public IQueryable<CustomerEntity> GetAll(int pageList, int pageNumber)
    {
        return _dbCustomer.Skip((pageNumber - 1) * pageList).Take(pageList);
    }

    public CustomerEntity FindById(int id)
    {
        var found = _dbCustomer.Find(id);
        return found;
    }

    public void Add(CustomerEntity entity)
    {
        _dbCustomer.Add(entity);
    }

    public void Update(CustomerEntity entity)
    {
        _dbCustomer.Update(entity);
    }

    public void Delete(CustomerEntity entity)
    {
        _dbCustomer.Remove(entity);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}