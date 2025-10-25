using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class EmployeeRepository: IEmployeeRepository
{
    private readonly DbSet<EmployeeEntity> _dbEmployee;
    private readonly QueueDbContext _context;
    
    public EmployeeRepository(QueueDbContext context)
    {
        _dbEmployee = context.Set<EmployeeEntity>();
        _context = context;
    }

    public IQueryable<EmployeeEntity> GetAll(int pageList, int pageNumber)
    {
        return _dbEmployee.Skip((pageNumber - 1) * pageList).Take(pageList);
    }

    public IQueryable<EmployeeEntity> GetAllEmployees()
    {
        return _dbEmployee
            .Include(s => s.Service.Company)
            .Include(s => s.Service)
            .Include(s=>s.Queues);
    }

    public IQueryable<EmployeeEntity> GetEmployeeByCompany(int companyId)
    {
        var found = _dbEmployee.Where(s => s.Service.CompanyId == companyId);
        return found;
    }

    public EmployeeEntity FindById(int id)
    {
        var found = _dbEmployee.Find(id);
        return found;
    }

    public void Add(EmployeeEntity entity)
    {
        _dbEmployee.Add(entity);
    }

    public void Update(EmployeeEntity entity)
    {
        _dbEmployee.Update(entity);
    }

    public void Delete(EmployeeEntity entity)
    {
        _dbEmployee.Remove(entity);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}