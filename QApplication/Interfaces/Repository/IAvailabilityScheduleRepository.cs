using QApplication.Requests.AvailabilityScheduleRequest;
using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IAvailabilityScheduleRepository
{
    IQueryable<AvailabilityScheduleEntity> GetAll(int pageList, int pageNumber);
    AvailabilityScheduleEntity FindById(int id);
   IQueryable<AvailabilityScheduleEntity>  GetEmployeeById(int employeeId);
    void Add(AvailabilityScheduleEntity entity);
    void Update(AvailabilityScheduleEntity entity);
    void Delete(AvailabilityScheduleEntity entity);
    int SaveChanges();
}