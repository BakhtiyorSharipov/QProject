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

    public CompanyEntity FindById(int id)
    {
        var found = _dbCompany.Find(id);
        return found;
    }

    public void Add(CompanyEntity entity)
    {
        _dbCompany.Add(entity);
    }

    public void Update(CompanyEntity entity)
    {
        _dbCompany.Update(entity);
    }

    public void Delete(CompanyEntity entity)
    {
        _dbCompany.Remove(entity);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}