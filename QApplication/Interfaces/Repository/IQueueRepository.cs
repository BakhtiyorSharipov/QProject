using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IQueueRepository
{
    IQueryable<QueueEntity> GetAll(int pageList, int pageNumber);
    QueueEntity FindById(int id);
    void Add(QueueEntity entity);
    void Update(QueueEntity entity);
    void Delete(QueueEntity entity);
    int SaveChanges();
}