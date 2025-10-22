using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IReviewRepository
{
    IQueryable<ReviewEntity> GetAll(int pageList, int pageNumber);
    IQueryable<ReviewEntity> GetAllReviewsByQueue(int queueId);
    IQueryable<ReviewEntity> GetAllReviewsForReport();
    ReviewEntity FindById(int id);
    void Add(ReviewEntity entity);
    int SaveChanges();
}