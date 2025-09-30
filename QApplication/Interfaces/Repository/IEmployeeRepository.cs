using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IEmployeeRepository
{
    IQueryable<EmployeeEntity> GetAll(int pageList, int pageNumber);
    EmployeeEntity FindById(int id);
    void Add(EmployeeEntity entity);
    void Update(EmployeeEntity entity);
    void Delete(EmployeeEntity entity);
    int SaveChanges();
}