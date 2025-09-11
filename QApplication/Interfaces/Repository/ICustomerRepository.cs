using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface ICustomerRepository 
{
    IQueryable<CustomerEntity> GetAll(int pageList, int pageNumber);
    CustomerEntity FindById(int id);
    void Add(CustomerEntity entity);
    void Update(CustomerEntity entity);
    void Delete(CustomerEntity entity);
    int SaveChanges();
}