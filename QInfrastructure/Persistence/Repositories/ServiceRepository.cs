using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class ServiceRepository:IServiceRepository
{
    private readonly DbSet<ServiceEntity> _dbService;
    private readonly QueueDbContext _context;
    
    public ServiceRepository(QueueDbContext context)
    {
        _dbService = context.Set<ServiceEntity>();
        _context = context;
    }

    public IQueryable<ServiceEntity> GetAll(int pageList, int pageNumber)
    {
        return _dbService.Skip((pageNumber - 1) * pageList).Take(pageList);
    }

    public IQueryable<ServiceEntity> GetAllServices()
    {
        return _dbService
            .Include(s => s.Company)
            .Include(s => s.Employees);
    }

    public IQueryable<ServiceEntity> GetAllServicesByCompany(int companyId)
    {
        return _dbService.Where(s => s.CompanyId == companyId);
    }

    public ServiceEntity FindById(int id)
    {
        var found = _dbService.Find(id);
        return found;
    }

    public void Add(ServiceEntity entity)
    {
        _dbService.Add(entity);
    }

    public void Update(ServiceEntity entity)
    {
        _dbService.Update(entity);
    }

    public void Delete(ServiceEntity entity)
    {
        _dbService.Remove(entity);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}