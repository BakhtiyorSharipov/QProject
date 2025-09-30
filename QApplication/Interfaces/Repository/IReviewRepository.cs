using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IReviewRepository
{
    IQueryable<ReviewEntity> GetAll(int pageList, int pageNumber);
    ReviewEntity FindById(int id);
    void Add(ReviewEntity entity);
    void Update(ReviewEntity entity);
    void Delete(ReviewEntity entity);
    int SaveChanges();
}