using System.Net;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Responses.ReportResponse;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Services;

public class ReportService: IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public ReportService(IReportRepository reportRepository, IEmployeeRepository employeeRepository)
    {
        _reportRepository = reportRepository;
        _employeeRepository = employeeRepository;
    }


    public EmployeeReportResponseModel GetEmployeeReport(int employeeId)
    {
        var employee = _employeeRepository.FindById(employeeId);
        if (employee==null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
        }

        var queues = _reportRepository.GetEmployeeReport(employee.Id).ToList();

        var completed = queues.Count(c => c.Status == QueueStatus.Completed);
        var canceledByEmployee = queues.Count(c => c.Status == QueueStatus.CancelledByEmployee);
        var canceledByCustomer = queues.Count(c => c.Status == QueueStatus.CancelledByCustomer);
        var canceledByAdmin = queues.Count(c => c.Status == QueueStatus.CanceledByAdmin);
        var pending = queues.Count(p => p.Status == QueueStatus.Pending);
        var didNotCome = queues.Count(d => d.Status == QueueStatus.DidNotCome);
        var confirmed = queues.Count(c => c.Status == QueueStatus.Confirmed);
        
        var canceled = canceledByAdmin + canceledByCustomer + canceledByEmployee;

        var totalQueues = completed + canceled + pending + didNotCome+ confirmed;

        var response = new EmployeeReportResponseModel
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.FirstName,
            TotalQueues = totalQueues,
            CompletedQueues = completed,
            CancelledQueues = canceled,
            PendingQueues = pending,
            DidNotComeQueues = didNotCome,
            Confirmed = confirmed
        };

        return response;

    }

    public QueueReportResponseModel GetQueueReport()
    {
        var queues = _reportRepository.GetQueueReport().ToList();

        var completed = queues.Count(s => s.Status == QueueStatus.Completed);
        var pending = queues.Count(s=>s.Status== QueueStatus.Pending);
        var canceledByCustomer = queues.Count(s=>s.Status== QueueStatus.CancelledByCustomer);
        var canceledByEmployee = queues.Count(s=>s.Status== QueueStatus.CancelledByEmployee);
        var didNotCome = queues.Count(s=>s.Status== QueueStatus.DidNotCome);

        var totalQueses = completed + pending + canceledByCustomer + canceledByEmployee + didNotCome;

        var response = new QueueReportResponseModel
        {
            TotalQueues = totalQueses,
            Completed = completed,
            Pending = pending,
            CancelledByCustomer = canceledByCustomer,
            CancelledByEmployee = canceledByEmployee,
            DidNotCome = didNotCome
        };

        return response;
    }

    public ComplaintReportResponseModel GetComplaintReport()
    {
        var complaints = _reportRepository.GetComplaintReport();

        var pending = complaints.Count(p => p.ComplaintStatus == ComplaintStatus.Pending);
        var reviewed = complaints.Count(r => r.ComplaintStatus == ComplaintStatus.Reviewed);
        var resolved = complaints.Count(r => r.ComplaintStatus == ComplaintStatus.Resolved);

        var totalComplaints = pending + reviewed + resolved;

        var response = new ComplaintReportResponseModel
        {
            TotalComplaints = totalComplaints,
            Pending = pending,
            Reviewed = reviewed,
            Resolved = resolved
        };

        return response;
    }

    public ReviewReportResponseModel GetReviewReport()
    {
        var reviews = _reportRepository.GetReviewReport().ToList();
        var rating1 = reviews.Count(r => r.Grade == 1);
        var rating2 = reviews.Count(r => r.Grade == 2);
        var rating3 = reviews.Count(r => r.Grade == 3);
        var rating4 = reviews.Count(r => r.Grade == 4);
        var rating5 = reviews.Count(r => r.Grade == 5);

        var totalReviews = rating1 + rating2 + rating3 + rating4 + rating5;
        var averageRating = (rating1 + (rating2 * 2) + (rating3 * 3) + (rating4 * 4) + (rating5 * 5)) / totalReviews;
        var response = new ReviewReportResponseModel
        {
            Rating1 = rating1,
            Rating2 = rating2,
            Rating3 = rating3,
            Rating4 = rating4,
            Rating5 = rating5,
            TotalReviews = totalReviews,
            AverageRating = averageRating
        };

        return response;
    }
}