using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IComplaintRepository
{
    IQueryable<ComplaintEntity> GetAllComplaints(int pageList, int pageNumber);
    IQueryable<ComplaintEntity> GetAllComplaintsByQueue(int id);

    IQueryable<ComplaintEntity> GetAllComplaintsByCompany(int companyId);
    IQueryable<ComplaintEntity> GetAllComplaintsForReport();
    Task<ComplaintEntity> FindComplaintByIdAsync(int id);    
    
    Task AddComplaintAsync(ComplaintEntity entity);
    void UpdateComplaintStatus(ComplaintEntity entity);
    Task<int> SaveChangesAsync();

}