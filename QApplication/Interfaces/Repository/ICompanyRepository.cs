using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface ICompanyRepository
{
    IQueryable<CompanyEntity> GetAll(int pageList, int pageNumber);
    IQueryable<CompanyEntity> GetAllCompanies();
    Task<CompanyEntity> FindByIdAsync(int id);
    Task AddAsync(CompanyEntity entity);
    void Update(CompanyEntity entity);
    void Delete(CompanyEntity entity);
    Task<int> SaveChangesAsync();
    
}