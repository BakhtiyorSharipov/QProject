using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class BlockedCustomerRepository : IBlockedCustomerRepository
{
    private readonly DbSet<BlockedCustomerEntity> _dbBlockedCustomer;
    private readonly QueueDbContext _context;

    public BlockedCustomerRepository(QueueDbContext context)
    {
        _dbBlockedCustomer = context.Set<BlockedCustomerEntity>();
        _context = context;
    }

    public IQueryable<BlockedCustomerEntity> GetAll(int pageList, int pageNumber)
    {
        return _dbBlockedCustomer.Skip((pageNumber - 1) * pageList).Take(pageList);
    }

    public IQueryable<BlockedCustomerEntity> GetAllBlockedCustomersByCompany(int companyId)
    {
        var found = _dbBlockedCustomer.Where(s => s.CompanyId == companyId);
        return found;
    }

    public BlockedCustomerEntity FindById(int id)
    {
        var foundEntity = _dbBlockedCustomer.Find(id);
        return foundEntity;
    }

    public void Add(BlockedCustomerEntity entity)
    {
        _dbBlockedCustomer.Add(entity);
    }
    

    public void Delete(BlockedCustomerEntity entity)
    {
        _dbBlockedCustomer.Remove(entity);
    }

    public bool Exists(int customerId, int companyId)
    {
        var customer = _dbBlockedCustomer.Where(s => s.CustomerId == customerId);
        var company = _dbBlockedCustomer.Where(s => s.CompanyId == companyId);

        if (customer.Any() && company.Any())
        {
            return true;
        }

        return false;
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}