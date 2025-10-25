using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class ReviewRepository: IReviewRepository
{
    private readonly DbSet<ReviewEntity> _dbReview;
    private readonly QueueDbContext _context;
    
    public ReviewRepository(QueueDbContext context)
    {
        _dbReview = context.Set<ReviewEntity>();
        _context = context;
    }

    public IQueryable<ReviewEntity> GetAll(int pageList, int pageNumber)
    {
        return _dbReview.Skip((pageNumber - 1) * pageList).Take(pageList);
    }

    public IQueryable<ReviewEntity> GetAllReviewsByQueue(int queueId)
    {
        return _dbReview.Where(q => q.QueueId == queueId);
    }

    public IQueryable<ReviewEntity> GetAllReviewsByCompany(int companyId)
    {
        var found = _dbReview.Where(s => s.Queue.Service.CompanyId == companyId);
        return found;
    }

    public IQueryable<ReviewEntity> GetAllReviewsForReport()
    {
        var reviews = _dbReview
            .Include(r => r.Customer)
            .Include(r => r.Queue)
            .Include(r=>r.Queue.Service.Company)
            .Include(s=>s.Queue.Employee)
            .ThenInclude(r => r.Service);
        return reviews;
    }

    public ReviewEntity FindById(int id)
    {
        var found = _dbReview.Find(id);
        return found;
    }

    public void Add(ReviewEntity entity)
    {
        _dbReview.Add(entity);
    }
    

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}