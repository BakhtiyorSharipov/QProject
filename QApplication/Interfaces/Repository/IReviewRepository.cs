using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IReviewRepository
{
    IQueryable<ReviewEntity> GetAll(int pageList, int pageNumber);
    IQueryable<ReviewEntity> GetAllReviewsByQueue(int queueId);
    IQueryable<ReviewEntity> GetAllReviewsByCompany(int companyId);
    IQueryable<ReviewEntity> GetAllReviewsForReport();
    ReviewEntity FindById(int id);
    void Add(ReviewEntity entity);
    int SaveChanges();
}