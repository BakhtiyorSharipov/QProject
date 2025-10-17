using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IComplaintRepository
{
    IQueryable<ComplaintEntity> GetAllComplaints(int pageList, int pageNumber);
    IQueryable<ComplaintEntity> GetAllComplaintsByQueue(int id);
    ComplaintEntity FindComplaintById(int id);
    void AddComplaint(ComplaintEntity entity);
    void UpdateComplaintStatus(ComplaintEntity entity);
    int SaveChanges();
}