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

    public async Task<ServiceEntity> FindByIdAsync(int id)
    {
        var found = await _dbService.FindAsync(id);
        return found;
    }

    public async Task AddAsync(ServiceEntity entity)
    {
        await _dbService.AddAsync(entity);
    }

    public void Update(ServiceEntity entity)
    {
        _dbService.Update(entity);
    }

    public void Delete(ServiceEntity entity)
    {
        _dbService.Remove(entity);
    }


    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}