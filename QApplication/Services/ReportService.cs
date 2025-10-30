using System.Net;
using System.Runtime.InteropServices;
using QApplication.Exceptions;
using QApplication.Interfaces;
using QApplication.Interfaces.Repository;
using QApplication.Requests.ReportRequest;
using QApplication.Responses;
using QApplication.Responses.ReportResponse;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IQueueRepository _queueRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IComplaintRepository _complaintRepository;
    private readonly IBlockedCustomerRepository _blockedCustomerRepository;

    public ReportService(IReportRepository reportRepository, IEmployeeRepository employeeRepository,
        ICustomerRepository customerRepository, ICompanyRepository companyRepository,
        IServiceRepository serviceRepository, IQueueRepository queueRepository, IReviewRepository reviewRepository,
        IComplaintRepository complaintRepository, IBlockedCustomerRepository blockedCustomerRepository)
    {
        _reportRepository = reportRepository;
        _employeeRepository = employeeRepository;
        _customerRepository = customerRepository;
        _companyRepository = companyRepository;
        _serviceRepository = serviceRepository;
        _queueRepository = queueRepository;
        _reviewRepository = reviewRepository;
        _complaintRepository = complaintRepository;
        _blockedCustomerRepository = blockedCustomerRepository;
    }


    public CompanyReportItemResponseModel GetCompanyReport(CompanyReportRequest request)
    {
        var company = _companyRepository.FindById(request.CompanyId);
        if (company== null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }
        
        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value> request.To.Value)
            {
                throw new Exception("'From' must be less than 'To'");
            }
        }
        
        var queues = _queueRepository.GetQueuesByCompany(request.CompanyId);
        var employees = _employeeRepository.GetEmployeeByCompany(request.CompanyId);
        var services = _serviceRepository.GetAllServicesByCompany(request.CompanyId);
        var customers = _customerRepository.GetAllCustomersByCompany(request.CompanyId);
        var reviews = _reviewRepository.GetAllReviewsByCompany(request.CompanyId);
        var complaints = _complaintRepository.GetAllComplaintsByCompany(request.CompanyId);
        var blockedCustomers = _blockedCustomerRepository.GetAllBlockedCustomersByCompany(request.CompanyId);
        
        
        if (request.From.HasValue)
        {
            queues = queues.Where(s => s.StartTime >= request.From.Value.ToUniversalTime());
        }

        if (request.To.HasValue)
        {
            queues = queues.Where(s => (s.EndTime.HasValue ? s.EndTime.Value : s.StartTime.AddHours(1)) <= request.To.Value.ToUniversalTime());
        }

        var totalQueues = queues.Count();
        var completedQueues = queues.Count(s => s.Status == QueueStatus.Completed);
        var cancelledQueues = queues.Count(q =>
            q.Status == QueueStatus.CancelledByCustomer || q.Status == QueueStatus.CancelledByEmployee);
        var didNotComeQueues = queues.Count(s => s.Status == QueueStatus.DidNotCome);

        double averageRating = 0;
        if (reviews.Any())
        {
            averageRating = reviews.Average(s => s.Grade);
        }

        var complaintCount = complaints.Count();
        var employeeCount = employees.Count();
        var serviceCount = services.Count();
        var customerCount = customers.Count();
        var blockedCustomersCount = blockedCustomers.Count();
        

        var mostPopularServices = queues
            .GroupBy(q => q.Service.ServiceName)
            .OrderByDescending(s => s.Count())
            .Select(g => g.Key)
            .Take(5)
            .ToList();

        var response = new CompanyReportItemResponseModel
        {
            CompanyId = company.Id,
            CompanyName = company.CompanyName,
            TotalQueues = totalQueues,
            CompletedCount = completedQueues,
            CancelledCount = cancelledQueues,
            DidNotCome = didNotComeQueues,
            AverageRating = averageRating,
            ComplaintCount = complaintCount,
            EmployeeCount = employeeCount,
            ServiceCount = serviceCount,
            TotalCustomers = customerCount,
            BlockedCustomers = blockedCustomersCount,
            MostPopularServices = mostPopularServices
        };

        return response;

    }

    public EmployeeReportResponseModel GetEmployeeReport(EmployeeReportRequest request)
    {
        var employees = _employeeRepository.GetAllEmployees();
        var totalEmployees = employees.Count();

        if (request.EmployeeId.HasValue)
        {
            var found = _employeeRepository.FindById(request.EmployeeId.Value);
            if (found==null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = _companyRepository.FindById(request.CompanyId.Value);
            if (found==null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = _serviceRepository.FindById(request.ServiceId.Value);
            if (found== null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }
        
        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value> request.To.Value)
            {
                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }
        
        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }
        
        if (request.EmployeeId.HasValue)
        {
            employees = employees.Where(s => s.Id == request.EmployeeId.Value);
        }

        if (request.CompanyId.HasValue)
        {
            employees = employees.Where(s => s.Service.CompanyId == request.CompanyId.Value);
        }

        if (request.ServiceId.HasValue)
        {
            employees = employees.Where(s => s.ServiceId == request.ServiceId.Value);
        }
        
        var employeeList = employees.ToList();

        var response = new EmployeeReportResponseModel();
        foreach (var employee in employeeList)
        {
            var queues = _queueRepository.GetQueuesByCustomer(employee.Id);

            if (request.From.HasValue)
            {
                queues = queues.Where(s => s.StartTime >= request.From.Value.ToUniversalTime());
            }

            if (request.To.HasValue)
            {
                queues = queues.Where(s =>
                    (s.EndTime.HasValue ? s.EndTime.Value : s.StartTime.AddHours(1)) <= request.To.Value.ToUniversalTime());
            }

            
            var completed = queues.Count(s => s.Status == QueueStatus.Completed);
            var pending = queues.Count(s => s.Status == QueueStatus.Pending);
            var confirmed = queues.Count(s => s.Status == QueueStatus.Confirmed);
            var canceledByEmployee = queues.Count(s => s.Status == QueueStatus.CancelledByEmployee);
            var canceledByCustomer = queues.Count(s => s.Status == QueueStatus.CancelledByCustomer);
            var didNotCome = queues.Count(s => s.Status == QueueStatus.DidNotCome);
            var total = completed + pending + confirmed + canceledByCustomer + canceledByEmployee + didNotCome;
           
            
            var employeeItem = new EmployeeReportItemResponseModel
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.FirstName,
                CompanyName = employee.Service.Company.CompanyName,
                ServiceName = employee.Service.ServiceName,
                TotalQueues = total,
                PendingQueues = pending,
                ConfirmedQueues = confirmed,
                CompletedQueues = completed,
                CancelledQueues = canceledByCustomer+canceledByEmployee,
                DidNotComeQueues = didNotCome,
            };
            
            response.Employees.Add(employeeItem);
            
        }

        response.TotalEmployees = totalEmployees;
        return response;
    }


    public QueueReportResponseModel GetQueueReport(QueueReportRequest request)
    {
        var queues = _reportRepository.GetQueueReport(request);


        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }

        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            var found = _employeeRepository.FindById(request.EmployeeId.Value);
            if (found == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = _companyRepository.FindById(request.CompanyId.Value);
            if (found == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = _serviceRepository.FindById(request.ServiceId.Value);
            if (found == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        if (request.From.HasValue)
        {
            queues = queues.Where(q => q.StartTime >= request.From.Value.ToUniversalTime());
        }

        if (request.To.HasValue)
        {
            queues = queues.Where(q =>
                (q.EndTime.HasValue ? q.EndTime.Value : q.StartTime.AddHours(1)) <=
                request.To.Value.ToUniversalTime());
        }


        if (request.EmployeeId.HasValue)
        {
            queues = queues.Where(q => q.EmployeeId == request.EmployeeId.Value);
        }

        if (request.ServiceId.HasValue)
        {
            queues = queues.Where(q => q.ServiceId == request.ServiceId.Value);
        }

        if (request.CompanyId.HasValue)
        {
            queues = queues.Where(q => q.Service.CompanyId == request.CompanyId.Value);
        }

        if (request.Status.HasValue)
        {
            queues = queues.Where(q => q.Status == request.Status.Value);
        }

        var queueList = queues.ToList();


        var queueItem = queueList.Select(queue => new QueueReportItemResponseModel
        {
            Id = queue.Id,
            EmployeeName = queue.Employee.FirstName,
            CompanyName = queue.Service.Company.CompanyName,
            ServiceName = queue.Service.ServiceName,
            CustomerName = queue.Customer.FirstName,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime.HasValue ? queue.EndTime.Value : queue.StartTime.AddHours(1),
            Status = queue.Status.ToString(),
        }).ToList();

        int totalQueues = 0,
            completedCount = 0,
            canceledCount = 0,
            didNotComeCount = 0,
            pendingCount = 0,
            confirmedCount = 0;

        if (request.IncludeStatistics)
        {
            completedCount = queueList.Count(c => c.Status == QueueStatus.Completed);
            int cancelledByCustomer = queueList.Count(c => c.Status == QueueStatus.CancelledByCustomer);
            int canceledByEmployee = queueList.Count(c => c.Status == QueueStatus.CancelledByEmployee);
            canceledCount = canceledByEmployee + cancelledByCustomer;
            didNotComeCount = queueList.Count(d => d.Status == QueueStatus.DidNotCome);
            pendingCount = queueList.Count(p => p.Status == QueueStatus.Pending);
            confirmedCount = queueList.Count(c => c.Status == QueueStatus.Confirmed);
            totalQueues = completedCount + canceledCount + didNotComeCount + pendingCount + confirmedCount;
        }

        var response = new QueueReportResponseModel
        {
            Queues = queueItem,
            TotalQueues = totalQueues,
            PendingCount = pendingCount,
            ConfirmedCount = confirmedCount,
            CompletedCount = completedCount,
            CanceledCount = canceledCount,
            DidNotComeCount = didNotComeCount
        };

        return response;
    }

    public ComplaintReportResponseModel GetComplaintReport(ComplaintReportRequest request)
    {
        var complaints = _reportRepository.GetComplaintReport(request);

        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }

        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            var found = _employeeRepository.FindById(request.EmployeeId.Value);
            if (found == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = _companyRepository.FindById(request.CompanyId.Value);
            if (found == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = _serviceRepository.FindById(request.ServiceId.Value);
            if (found == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        if (request.From.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.StartTime >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            complaints = complaints.Where(c =>
                (c.Queue.EndTime.HasValue ? c.Queue.EndTime.Value : c.Queue.StartTime.AddHours(1)) <=
                request.To.Value);
        }

        if (request.EmployeeId.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.EmployeeId == request.EmployeeId.Value);
        }

        if (request.CompanyId.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.Service.CompanyId == request.CompanyId.Value);
        }

        if (request.ServiceId.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.ServiceId == request.ServiceId.Value);
        }

        if (request.Status.HasValue)
        {
            complaints = complaints.Where(q => q.ComplaintStatus == request.Status.Value);
        }

        var complaintList = complaints.ToList();
        var complaintItem = complaintList.Select(complaint => new ComplaintReportItemResponseModel
        {
            Id = complaint.Id,
            EmployeeName = complaint.Queue.Employee.FirstName,
            CompanyName = complaint.Queue.Service.Company.CompanyName,
            ServiceName = complaint.Queue.Service.ServiceName,
            CustomerName = complaint.Customer.FirstName,
            Status = complaint.ComplaintStatus.ToString(),
            ComplaintText = complaint.ComplaintText,
            ResponseText = complaint.ResponseText
        }).ToList();

        int totalComplaints = 0, pending = 0, reviewed = 0, resolved = 0;

        if (request.IncludeStatistics)
        {
            pending = complaints.Count(p => p.ComplaintStatus == ComplaintStatus.Pending);
            reviewed = complaints.Count(r => r.ComplaintStatus == ComplaintStatus.Reviewed);
            resolved = complaints.Count(r => r.ComplaintStatus == ComplaintStatus.Resolved);

            totalComplaints = pending + reviewed + resolved;
        }


        var response = new ComplaintReportResponseModel
        {
            Complaints = complaintItem,
            TotalComplaints = totalComplaints,
            PendingCount = pending,
            ReviewedCount = reviewed,
            ResolvedCount = resolved
        };

        return response;
    }

    public ReviewReportResponseModel GetReviewReport(ReviewReportRequest request)
    {
        var reviews = _reportRepository.GetReviewReport(request);

        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }

        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }


        if (request.EmployeeId.HasValue)
        {
            var found = _employeeRepository.FindById(request.EmployeeId.Value);

            if (found == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = _companyRepository.FindById(request.CompanyId.Value);
            if (found == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = _serviceRepository.FindById(request.ServiceId.Value);
            if (found == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        if (request.From.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.StartTime >= request.From.Value.ToUniversalTime());
        }

        if (request.To.HasValue)
        {
            reviews = reviews.Where(s =>
                (s.Queue.EndTime.HasValue ? s.Queue.EndTime.Value : s.Queue.StartTime.AddHours(1)) <=
                request.To.Value.ToUniversalTime());
        }

        if (request.EmployeeId.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.EmployeeId == request.EmployeeId.Value);
        }

        if (request.CompanyId.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.Service.CompanyId == request.CompanyId.Value);
        }

        if (request.ServiceId.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.ServiceId == request.ServiceId.Value);
        }


        var reviewList = reviews.ToList();

        var reviewItem = reviewList.Select(review => new ReviewReportItemResponseModel
        {
            Id = review.Id,
            QueueId = review.QueueId,
            EmployeeName = review.Queue.Employee.FirstName,
            CompanyName = review.Queue.Service.Company.CompanyName,
            CustomerName = review.Customer.FirstName,
            ServiceName = review.Queue.Service.ServiceName,
            Grade = review.Grade,
            ReviewText = review.ReviewText
        }).ToList();

        int totalReviews = 0, rating1 = 0, rating2 = 0, rating3 = 0, rating4 = 0, rating5 = 0;
        double average = 0;

        if (request.IncludeStatistics)
        {
            rating1 = reviews.Count(r => r.Grade == 1);
            rating2 = reviews.Count(r => r.Grade == 2);
            rating3 = reviews.Count(r => r.Grade == 3);
            rating4 = reviews.Count(r => r.Grade == 4);
            rating5 = reviews.Count(r => r.Grade == 5);

            totalReviews = rating1 + rating2 + rating3 + rating4 + rating5;
            average = (rating1 + (rating2 * 2) + (rating3 * 3) + (rating4 * 4) + (rating5 * 5)) / totalReviews;
        }


        var response = new ReviewReportResponseModel
        {
            Reviews = reviewItem,
            Rating1 = rating1,
            Rating2 = rating2,
            Rating3 = rating3,
            Rating4 = rating4,
            Rating5 = rating5,
            TotalReviews = totalReviews,
            AverageRating = average
        };

        return response;
    }

    public ServiceReportResponseModel GetServiceReport(ServiceReportRequest request)
    {
        var services = _reportRepository.GetServiceReport(request);
        var totalServices = 0;

        if (request.CompanyId.HasValue)
        {
            var found = _companyRepository.FindById(request.CompanyId.Value);
            if (found== null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = _serviceRepository.FindById(request.ServiceId.Value);
            if (found== null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            if (request.CompanyId.HasValue)
            {
                throw new Exception("ServiceId cannot be used together with CompanyId.");
            }
        }
        
        if (request.CompanyId.HasValue)
        {
            services = services.Where(s => s.CompanyId == request.CompanyId.Value);
            totalServices = services.Count();
        }

        if (request.ServiceId.HasValue)
        {
            services = services.Where(s => s.Id == request.ServiceId.Value);
        }

        var serviceList = services.ToList();
        var response = new ServiceReportResponseModel();
        foreach (var service in serviceList)
        {
            var queues = _queueRepository.GetQueuesByService(service.Id);

            var pending = queues.Count(s => s.Status == QueueStatus.Pending);
            var confirmed = queues.Count(s => s.Status == QueueStatus.Confirmed);
            var completed = queues.Count(s => s.Status == QueueStatus.Completed);
            var cancelledByCustomer = queues.Count(s => s.Status == QueueStatus.CancelledByCustomer);
            var cancelledByEmployee = queues.Count(s => s.Status == QueueStatus.CancelledByEmployee);
            var didNotCome = queues.Count(s => s.Status == QueueStatus.DidNotCome);
            var cancelled = cancelledByCustomer + cancelledByEmployee;
            var totalQueues = pending + confirmed + completed + cancelledByCustomer + cancelledByEmployee + didNotCome;

            var serviceItem = new ServiceReportItemResponseModel
            {
                ServiceId = service.Id,
                ServiceName = service.ServiceName,
                CompanyName = service.Company.CompanyName,
                PendingCount = pending,
                ConfirmedCount = confirmed,
                CompletedCount = completed,
                CancelledCount = cancelled,
                DidNotCount = didNotCome,
                TotalQueues = totalQueues
            };
            
            response.Services.Add(serviceItem);
        }

        response.TotalServices = totalServices;

        return response;


    }
}