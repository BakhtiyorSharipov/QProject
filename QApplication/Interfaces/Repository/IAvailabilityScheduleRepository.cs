using QApplication.Requests.AvailabilityScheduleRequest;
using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IAvailabilityScheduleRepository
{
    IQueryable<AvailabilityScheduleEntity> GetAll(int pageList, int pageNumber);
    Task<AvailabilityScheduleEntity> FindByIdAsync(int id);
    IQueryable<AvailabilityScheduleEntity>  GetEmployeeById(int employeeId);
    IQueryable<AvailabilityScheduleEntity> GetAllSchedules();
    Task AddAsync(AvailabilityScheduleEntity entity);
    void Update(AvailabilityScheduleEntity entity);
    void Delete(AvailabilityScheduleEntity entity);
    Task<int> SaveChangesAsync();
}