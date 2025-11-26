using QApplication.Interfaces.Repository;
using QApplication.Requests.ReportRequest;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;


namespace QInfrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly IQueueRepository _queueRepository;
    private readonly IComplaintRepository _complaintRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IServiceRepository _serviceRepository;

    public ReportRepository(IQueueRepository queueRepository, IComplaintRepository complaintRepository,
        IReviewRepository reviewRepository, ICompanyRepository companyRepository, IEmployeeRepository employeeRepository, IServiceRepository serviceRepository)
    {
        _queueRepository = queueRepository;
        _complaintRepository = complaintRepository;
        _reviewRepository = reviewRepository;
        _companyRepository = companyRepository;
        _employeeRepository = employeeRepository;
        _serviceRepository = serviceRepository;
    }
    

    public IQueryable<EmployeeEntity> GetEmployeeReport(EmployeeReportRequest request)
    {
        return _employeeRepository.GetAllEmployees();
    }
    
    public IQueryable<QueueEntity> GetQueueReport(QueueReportRequest request)
    {
        return  _queueRepository.GetAllQueues();
    }

    public IQueryable<ComplaintEntity> GetComplaintReport(ComplaintReportRequest request)
    {
        return _complaintRepository.GetAllComplaintsForReport();
    }

    public IQueryable<ReviewEntity> GetReviewReport(ReviewReportRequest request)
    {
        return _reviewRepository.GetAllReviewsForReport();
    }

    public IQueryable<ServiceEntity> GetServiceReport(ServiceReportRequest request)
    {
        return _serviceRepository.GetAllServices();
    }
    
}

