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

    public IQueryable<CustomerEntity> GetAllCustomersByCompany(int companyId)
    {
        var found = _dbCustomer
            .Where(c => c.Queues.Any(q => q.Service.CompanyId == companyId));

        return found;
    }


    public async Task<CustomerEntity> FindByIdAsync(int id)
    {
        var found = await _dbCustomer.FindAsync(id);
        return found;
    }
    

    public async Task AddAsync(CustomerEntity entity)
    {
        await _dbCustomer.AddAsync(entity);
    }

    public void Update(CustomerEntity entity)
    {
        _dbCustomer.Update(entity);
    }

    public void Delete(CustomerEntity entity)
    {
        _dbCustomer.Remove(entity);
    }
    

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}