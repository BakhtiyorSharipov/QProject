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

    public async Task<ReviewEntity> FindByIdAsync(int id)
    {
        var found = await _dbReview.FindAsync(id);
        return found;
    }
    

    public async Task AddAsync(ReviewEntity entity)
    {
        await _dbReview.AddAsync(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}