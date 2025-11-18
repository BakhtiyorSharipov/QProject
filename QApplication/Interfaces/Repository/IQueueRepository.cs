using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IQueueRepository
{
    IQueryable<QueueEntity> GetAll(int pageList, int pageNumber);
    Task<QueueEntity> FindByIdAsync(int id);
    Task AddAsync(QueueEntity entity);
    void Update(QueueEntity entity);
    void Delete(QueueEntity entity);
    Task<int> SaveChangesAsync();

    IQueryable<QueueEntity> GetQueuesByCustomer(int customerId);
    IQueryable<QueueEntity> GetQueuesByEmployee(int employeeId);
    IQueryable<QueueEntity> GetQueuesByService(int serviceId);
    IQueryable<QueueEntity> GetQueuesByCompany(int companyId);
    IQueryable<QueueEntity> GetAllQueues();

    

}