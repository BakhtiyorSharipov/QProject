using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class ComplaintRepository: IComplaintRepository
{
    private readonly DbSet<ComplaintEntity> _dbComplaint;
    private readonly QueueDbContext _context;

    public ComplaintRepository(QueueDbContext context)
    {
        _dbComplaint = context.Set<ComplaintEntity>();
        _context = context;
    }
    
    public IQueryable<ComplaintEntity> GetAllComplaints(int pageList, int pageNumber)
    {
        return _dbComplaint.Skip((pageNumber - 1) * pageList).Take(pageList);
    }

    public IQueryable<ComplaintEntity> GetAllComplaintsByQueue(int id)
    {
        return _dbComplaint.Where(q => q.QueueId == id);
    }

    public IQueryable<ComplaintEntity> GetAllComplaintsForReport()
    {
        var complaints = _dbComplaint
            .Include(c => c.Customer)
            .Include(c => c.Queue);

        return complaints;
    }

    public ComplaintEntity FindComplaintById(int id)
    {
        var found = _dbComplaint.Find(id);
        return found;
    }

    public void AddComplaint(ComplaintEntity entity)
    {
        _dbComplaint.Add(entity);
    }

    public void UpdateComplaintStatus(ComplaintEntity entity)
    {
        _dbComplaint.Update(entity);
    }
    
    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}