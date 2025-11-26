using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface ICustomerRepository
{
    IQueryable<CustomerEntity> GetAll(int pageList, int pageNumber);
    IQueryable<CustomerEntity> GetAllCustomersByCompany(int companyId);
    Task<CustomerEntity> FindByIdAsync(int id);

    Task AddAsync(CustomerEntity entity);
    void Update(CustomerEntity entity);
    void Delete(CustomerEntity entity);

    Task<int> SaveChangesAsync();
}