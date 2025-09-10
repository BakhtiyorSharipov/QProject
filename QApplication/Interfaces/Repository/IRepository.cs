using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IRepository<TEntity> where TEntity: BaseEntity
{
    IQueryable<TEntity> GetAll(int pageList, int pageNumber);
    TEntity FindById(int id);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    int SaveChanges();
}