using QApplication.Interfaces.Repository;
using QDomain.Models;


namespace QInfrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly IQueueRepository _queueRepository;
    private readonly IComplaintRepository _complaintRepository;
    private readonly IReviewRepository _reviewRepository;


    public ReportRepository(IQueueRepository queueRepository, IComplaintRepository complaintRepository,
        IReviewRepository reviewRepository)
    {
        _queueRepository = queueRepository;
        _complaintRepository = complaintRepository;
        _reviewRepository = reviewRepository;
    }

    public IQueryable<QueueEntity> GetEmployeeReport(int employeeId)
    {
        return _queueRepository.GetQueuesByEmployee(employeeId);
    }

    public IQueryable<QueueEntity> GetQueueReport()
    {
        return _queueRepository.GetAllQueues();
    }

    public IQueryable<ComplaintEntity> GetComplaintReport()
    {
        return _complaintRepository.GetAllComplaintsForReport();
    }

    public IQueryable<ReviewEntity> GetReviewReport()
    {
        return _reviewRepository.GetAllReviewsForReport();
    }
}