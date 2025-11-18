using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IEmployeeRepository
{
    IQueryable<EmployeeEntity> GetAll(int pageList, int pageNumber);
    IQueryable<EmployeeEntity> GetAllEmployees();
    IQueryable<EmployeeEntity> GetEmployeeByCompany(int companyId);
    Task<EmployeeEntity> FindByIdAsync(int id);
    Task AddAsync(EmployeeEntity entity);
    void Update(EmployeeEntity entity);
    void Delete(EmployeeEntity entity);
    Task<int> SaveChangesAsync();
}