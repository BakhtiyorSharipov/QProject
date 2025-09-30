using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IServiceRepository
{
    IQueryable<ServiceEntity> GetAll(int pageList, int pageNumber);
    ServiceEntity FindById(int id);
    void Add(ServiceEntity entity);
    void Update(ServiceEntity entity);
    void Delete(ServiceEntity entity);
    int SaveChanges();
}