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

    public AvailabilityScheduleEntity FindById(int id)
    {
        var foundSchedule = _dbAvailabilitySchedule.Find(id);
        return foundSchedule;
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

    public void Add(AvailabilityScheduleEntity entity)
    {
        _dbAvailabilitySchedule.Add(entity);
    }

    public void Update(AvailabilityScheduleEntity entity)
    {
        _dbAvailabilitySchedule.Update(entity);
    }

    public void Delete(AvailabilityScheduleEntity entity)
    {
        _dbAvailabilitySchedule.Remove(entity);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}