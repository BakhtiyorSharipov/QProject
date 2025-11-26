using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IServiceRepository
{
    IQueryable<ServiceEntity> GetAll(int pageList, int pageNumber);
    IQueryable<ServiceEntity> GetAllServices();
    IQueryable<ServiceEntity> GetAllServicesByCompany(int companyId);
    Task<ServiceEntity> FindByIdAsync(int id);
    Task AddAsync(ServiceEntity entity);
    void Update(ServiceEntity entity);
    void Delete(ServiceEntity entity);
    Task<int> SaveChangesAsync();
}