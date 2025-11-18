using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly DbSet<CompanyEntity> _dbCompany;
    private readonly QueueDbContext _context;

    public CompanyRepository(QueueDbContext context)
    {
        _dbCompany = context.Set<CompanyEntity>();
        _context = context;
    }

    public IQueryable<CompanyEntity> GetAll(int pageList, int pageNumber)
    {
        return _dbCompany.Skip((pageNumber - 1) * pageList).Take(pageList);
    }

   

    public IQueryable<CompanyEntity> GetAllCompanies()
    {
        return _dbCompany
            .Include(s => s.Services)
                .ThenInclude(s => s.Employees)
                .ThenInclude(s => s.Queues)
                .ThenInclude(s => s.Customer)
                .ThenInclude(s => s.Reviews)
                .ThenInclude(s => s.Customer.Complaints);
    }

    public async Task<CompanyEntity> FindByIdAsync(int id)
    {
        var found = await _dbCompany.FindAsync(id);
        return found;
    }

   

    public async Task AddAsync(CompanyEntity entity)
    {
       await _dbCompany.AddAsync(entity);
    }

    public void Update(CompanyEntity entity)
    {
        _dbCompany.Update(entity);
    }
    

    public void Delete(CompanyEntity entity)
    {
        _dbCompany.Remove(entity);
    }
    
    

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}