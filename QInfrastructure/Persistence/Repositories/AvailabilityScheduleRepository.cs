using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class AvailabilityScheduleRepository : IAvailabilityScheduleRepository
{
    private readonly DbSet<AvailabilityScheduleEntity> _dbAvailabilitySchedule;
    private readonly QueueDbContext _context;

    public AvailabilityScheduleRepository(QueueDbContext context)
    {
        _dbAvailabilitySchedule = context.Set<AvailabilityScheduleEntity>();
        _context = context;
    }

    public IQueryable<AvailabilityScheduleEntity> GetAll(int pageList, int pageNumber)
    {
        return _dbAvailabilitySchedule.Skip((pageNumber - 1) * pageList).Take(pageList);
    }
    
    public async Task<AvailabilityScheduleEntity> FindByIdAsync(int id)
    {
        var found = await _dbAvailabilitySchedule.FindAsync(id);
        return found;
    }

    public IQueryable<AvailabilityScheduleEntity> GetEmployeeById(int employeeId)
    {
        var foundEntity = _dbAvailabilitySchedule.Where(s => s.EmployeeId == employeeId);
        return foundEntity;
    }

    public IQueryable<AvailabilityScheduleEntity> GetAllSchedules()
    {
        return _dbAvailabilitySchedule;
    }
    
    public async Task AddAsync(AvailabilityScheduleEntity entity)
    {
        await _dbAvailabilitySchedule.AddAsync(entity);
    }

    public void Update(AvailabilityScheduleEntity entity)
    {
        _dbAvailabilitySchedule.Update(entity);
    }

    public void Delete(AvailabilityScheduleEntity entity)
    {
        _dbAvailabilitySchedule.Remove(entity);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}