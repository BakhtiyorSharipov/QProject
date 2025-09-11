using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class ReviewRepository: IReviewRepository
{
    private readonly DbSet<ReviewEntity> _dbReview;
    private readonly EFContext _context;
    
    public ReviewRepository(EFContext context)
    {
        _dbReview = context.Set<ReviewEntity>();
        _context = context;
    }

    public IQueryable<ReviewEntity> GetAll(int pageList, int pageNumber)
    {
        return _dbReview.Skip((pageNumber - 1) * pageList).Take(pageList);
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

    public void Update(ReviewEntity entity)
    {
        _dbReview.Update(entity);
    }

    public void Delete(ReviewEntity entity)
    {
        _dbReview.Remove(entity);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}