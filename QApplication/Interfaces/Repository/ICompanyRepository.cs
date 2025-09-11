using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface ICompanyRepository
{
    IQueryable<CompanyEntity> GetAll(int pageList, int pageNumber);
    CompanyEntity FindById(int id);
    void Add(CompanyEntity entity);
    void Update(CompanyEntity entity);
    void Delete(CompanyEntity entity);
    int SaveChanges();
}