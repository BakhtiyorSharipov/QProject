using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<ReportService> _logger;

    public ReportService(IReportRepository reportRepository, IEmployeeRepository employeeRepository,
        ICustomerRepository customerRepository, ICompanyRepository companyRepository,
        IServiceRepository serviceRepository, IQueueRepository queueRepository, IReviewRepository reviewRepository,
        IComplaintRepository complaintRepository, IBlockedCustomerRepository blockedCustomerRepository, ILogger<ReportService> logger)
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
        _logger = logger;
    }


    public CompanyReportItemResponseModel GetCompanyReport(CompanyReportRequest request)
    {
       
        _logger.LogInformation("Starting company report for CompanyId: {id} with date range: {from} to {to}", request.CompanyId, request.From, request.To);
        var company = _companyRepository.FindById(request.CompanyId);
        if (company== null)
        {
            _logger.LogWarning("Company with Id {id} not found for getting company report.", request.CompanyId );
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }
        
        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value> request.To.Value)
            {
                _logger.LogError("Invalid date range for company report. CompanyId: {CompanyId}, From: {From}, To: {To}", request.CompanyId,request.From, request.To);
                throw new Exception("'From' must be less than 'To'");
            }
        }
        
        _logger.LogDebug("Fetching data for company with Id {id} report.", request.CompanyId);
        var queues = _queueRepository.GetQueuesByCompany(request.CompanyId);
        var employees = _employeeRepository.GetEmployeeByCompany(request.CompanyId);
        var services = _serviceRepository.GetAllServicesByCompany(request.CompanyId);
        var customers = _customerRepository.GetAllCustomersByCompany(request.CompanyId);
        var reviews = _reviewRepository.GetAllReviewsByCompany(request.CompanyId);
        var complaints = _complaintRepository.GetAllComplaintsByCompany(request.CompanyId);
        var blockedCustomers = _blockedCustomerRepository.GetAllBlockedCustomersByCompany(request.CompanyId);
        
        _logger.LogDebug("Base data - Queues: {queueCount}, Employees: {employeeCount}, Services: {serviceCount}", queues.Count(), employees.Count(),services.Count());
        if (request.From.HasValue)
        {
            _logger.LogDebug("Data filtering From: {from}, Current queues: {queues}", request.From.Value.ToUniversalTime(), queues.Count());
            queues = queues.Where(s => s.StartTime >= request.From.Value.ToUniversalTime());
            _logger.LogDebug("After From filter: {queues} queues", queues.Count());
        }

        if (request.To.HasValue)
        {
            _logger.LogDebug("Data filtering To: {to}, Current queues: {queues}", request.To.Value.ToUniversalTime(), queues.Count());
            queues = queues.Where(s => (s.EndTime.HasValue ? s.EndTime.Value : s.StartTime.AddMinutes(30)) <= request.To.Value.ToUniversalTime());
            _logger.LogDebug("After To filter: {queues} queues", queues.Count());
        }

        var totalQueues = queues.Count();
        var completedQueues = queues.Count(s => s.Status == QueueStatus.Completed);
        var cancelledQueues = queues.Count(q =>
            q.Status == QueueStatus.CancelledByCustomer || q.Status == QueueStatus.CancelledByEmployee);
        var didNotComeQueues = queues.Count(s => s.Status == QueueStatus.DidNotCome);

        _logger.LogDebug("Queue statistics - Total: {totalQueues}, Completed: {Completed}, Cancelled: {cancelled}, DidNotCome: {didNotCome}", totalQueues, completedQueues, cancelledQueues,didNotComeQueues);
        
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
        _logger.LogDebug("Calculated - AverageRating: {avRa}, Complaints: {complaints}, PopularServices: {popularServices}", averageRating, complaintCount, mostPopularServices.Count);

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
        
        _logger.LogInformation("Company report completed.");
        return response;

    }

    public EmployeeReportResponseModel GetEmployeeReport(EmployeeReportRequest request)
    {
        _logger.LogInformation("Starting employee report with filters - EmployeeId: {EmployeeId}, CompanyId: {CompanyId}, ServiceId: {ServiceId}", request.EmployeeId, request.CompanyId, request.ServiceId);
        var employees = _employeeRepository.GetAllEmployees();
        var totalEmployees = employees.Count();
        
        _logger.LogDebug("Employee count: {employeeCount}", totalEmployees);
        
        if (request.EmployeeId.HasValue)
        {
            var found = _employeeRepository.FindById(request.EmployeeId.Value);
            if (found==null)
            {
                _logger.LogWarning("Employee with Id {id} not found for getting report.", request.EmployeeId);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = _companyRepository.FindById(request.CompanyId.Value);
            if (found==null)
            {
                _logger.LogWarning("Company with Id {id} not found for getting employee report.", request.CompanyId);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = _serviceRepository.FindById(request.ServiceId.Value);
            if (found== null)
            {
                _logger.LogWarning("Service with Id {id} not found for getting employee report.", request.ServiceId);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }
        
        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value> request.To.Value)
            {
                _logger.LogError("Invalid date range for employee report. From: {From}, To: {To}", request.From, request.To);
                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError("Invalid parameter combination for employee report. EmployeeId cannot be used with CompanyId or ServiceId");
                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }
        
        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError("Invalid parameter combination for employee report. CompanyId cannot be used with EmployeeId or ServiceId");
                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }
        
        if (request.EmployeeId.HasValue)
        {
            _logger.LogDebug("Filtering by EmployeeId {id}", request.EmployeeId.Value);
            employees = employees.Where(s => s.Id == request.EmployeeId.Value);
        }

        if (request.CompanyId.HasValue)
        {
            _logger.LogDebug("Filtering by CompanyId {id}", request.CompanyId.Value);
            employees = employees.Where(s => s.Service.CompanyId == request.CompanyId.Value);
        }

        if (request.ServiceId.HasValue)
        {
            _logger.LogDebug("Filtering by ServiceId {id}", request.ServiceId.Value);
            employees = employees.Where(s => s.ServiceId == request.ServiceId.Value);
        }
        
        
        
        var employeeList = employees.ToList();
        _logger.LogDebug("After filtering employees {employeeCount}", employeeList.Count);
        
        var response = new EmployeeReportResponseModel();
        foreach (var employee in employeeList)
        {
            var queues = _queueRepository.GetQueuesByCustomer(employee.Id);

            if (request.From.HasValue)
            {
                queues = queues.Where(s => s.StartTime >= request.From.Value.ToUniversalTime());
                _logger.LogDebug("After From filter for EmployeeId {id}: {queueCount} queues.", employee.Id, queues.Count());

            }

            if (request.To.HasValue)
            {
                queues = queues.Where(s =>
                    (s.EndTime.HasValue ? s.EndTime.Value : s.StartTime.AddMinutes(30)) <= request.To.Value.ToUniversalTime());
                _logger.LogDebug("After To filter for EmployeeId {id}: {queueCount} queues.", employee.Id, queues.Count());

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
            _logger.LogInformation("Employee report item successfully added to employee report response model.");
        }

        response.TotalEmployees = totalEmployees;
        _logger.LogInformation("Employee report completed.");
        return response;
        
    }


    public QueueReportResponseModel GetQueueReport(QueueReportRequest request)
    {
        
        _logger.LogInformation("Starting queue report with filters -CompanyId: {companyId}, EmployeeId: {employeeId}, ServiceId: {serviceId}, Status: {status}"
            , request.CompanyId, request.EmployeeId, request.ServiceId, request.Status);
        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                _logger.LogError("Invalid date range for queue report. From: {From}, To: {To}", request.From, request.To);
                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError("Invalid parameter combination for queue report. EmployeeId cannot be used with CompanyId or ServiceId");
                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }

        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError("Invalid parameter combination for queue report. CompanyId cannot be used with EmployeeId or ServiceId");
                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            var found = _employeeRepository.FindById(request.EmployeeId.Value);
            if (found == null)
            {
                _logger.LogWarning("Employee with Id {id} not found for getting queue report.", request.EmployeeId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = _companyRepository.FindById(request.CompanyId.Value);
            if (found == null)
            {
                _logger.LogWarning("Company with Id {id} not found for getting queue report.", request.CompanyId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = _serviceRepository.FindById(request.ServiceId.Value);
            if (found == null)
            {
                _logger.LogWarning("Service with Id {id} not found for getting queue report.", request.ServiceId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        var queues = _reportRepository.GetQueueReport(request);
        _logger.LogDebug("{queueCount} queues from repository", queues.Count()); 
        
        
        if (request.From.HasValue)
        {
            queues = queues.Where(q => q.StartTime >= request.From.Value.ToUniversalTime());
            _logger.LogDebug("After From filter: {queueCount} queues", queues.Count());
        }

        if (request.To.HasValue)
        {
            queues = queues.Where(q =>
                (q.EndTime.HasValue ? q.EndTime.Value : q.StartTime.AddMinutes(30)) <=
                request.To.Value.ToUniversalTime());
            _logger.LogDebug("After To filter: {queueCount} queues", queues.Count());

        }


        if (request.EmployeeId.HasValue)
        {
            queues = queues.Where(q => q.EmployeeId == request.EmployeeId.Value);
            _logger.LogDebug("After EmployeeId {id} filter: {queueCount} queues",request.EmployeeId.Value, queues.Count());

        }

        if (request.ServiceId.HasValue)
        {
            queues = queues.Where(q => q.ServiceId == request.ServiceId.Value);
            _logger.LogDebug("After ServiceId {id} filter: {queueCount} queues",request.ServiceId.Value, queues.Count());

        }

        if (request.CompanyId.HasValue)
        {
            queues = queues.Where(q => q.Service.CompanyId == request.CompanyId.Value);
            _logger.LogDebug("After CompanyId {id} filter: {queueCount} queues",request.CompanyId.Value, queues.Count());

        }

        if (request.Status.HasValue)
        {
            queues = queues.Where(q => q.Status == request.Status.Value);
            _logger.LogDebug("After Status {Status} filter: {queueCount} queues",request.Status.Value, queues.Count());

        }

        var queueList = queues.ToList();
        _logger.LogDebug("Final queue count {queueCount}", queueList.Count);

        var queueItem = queueList.Select(queue => new QueueReportItemResponseModel
        {
            Id = queue.Id,
            EmployeeName = queue.Employee.FirstName,
            CompanyName = queue.Service.Company.CompanyName,
            ServiceName = queue.Service.ServiceName,
            CustomerName = queue.Customer.FirstName,
            StartTime = queue.StartTime,
            EndTime = queue.EndTime.HasValue ? queue.EndTime.Value : queue.StartTime.AddMinutes(30),
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

        _logger.LogDebug("Queue statistics - Total: {totalQueues}, Completed: {compelted}, Pending: {pending}, Confirmed: {confirmed}, Cancelled: {cancelled}, DidNotCome: {didNotCome}"
            , totalQueues, completedCount, pendingCount, confirmedCount, canceledCount, didNotComeCount );
        
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

        _logger.LogInformation("Queue report completed.");
        return response;
    }

    public ComplaintReportResponseModel GetComplaintReport(ComplaintReportRequest request)
    {
        _logger.LogInformation("Getting complaint report with filters - CompanyId: {CompanyId}, EmployeeId: {EmployeeId}, ServiceId: {ServiceId}, Status: {Status}.", 
            request.CompanyId, request.EmployeeId, request.ServiceId, request.Status);
        var complaints = _reportRepository.GetComplaintReport(request);
        _logger.LogDebug("{complaintCount} complaints from repository", complaints.Count());
        
        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                _logger.LogError("Invalid date range for complaint report. From: {From}, To: {To}", request.From, request.To);

                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError("Invalid parameter combination for complaint report. EmployeeId cannot be used with CompanyId or ServiceId");

                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }

        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError("Invalid parameter combination for complaint report. CompanyId cannot be used with EmployeeId or ServiceId");

                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            var found = _employeeRepository.FindById(request.EmployeeId.Value);
            if (found == null)
            {
                _logger.LogWarning("Employee with Id {id} not found for getting complaint report.", request.EmployeeId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = _companyRepository.FindById(request.CompanyId.Value);
            if (found == null)
            {
                _logger.LogWarning("Company with Id {id} not found for getting complaint report.", request.CompanyId.Value);

                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = _serviceRepository.FindById(request.ServiceId.Value);
            if (found == null)
            {
                _logger.LogWarning("Service with Id {id} not found for getting complaint report.", request.ServiceId.Value);

                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        _logger.LogDebug("Applying filter: {compalintCount} complaints.", complaints.Count());
        if (request.From.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.StartTime >= request.From.Value);
            _logger.LogDebug("After From filter: {complaintCount} complaints", complaints.Count());
        }

        if (request.To.HasValue)
        {
            complaints = complaints.Where(c =>
                (c.Queue.EndTime.HasValue ? c.Queue.EndTime.Value : c.Queue.StartTime.AddMinutes(30)) <=
                request.To.Value);
            _logger.LogDebug("After To filter: {complaintCount} complaints", complaints.Count());

        }

        if (request.EmployeeId.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.EmployeeId == request.EmployeeId.Value);
            _logger.LogDebug("After EmployeeId {id} filter: {complaintCount} complaints", complaints.Count(), request.EmployeeId.Value);

        }

        if (request.CompanyId.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.Service.CompanyId == request.CompanyId.Value);
            _logger.LogDebug("After CompanyId {id} filter: {complaintCount} complaints", complaints.Count(), request.CompanyId.Value);

        }

        if (request.ServiceId.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.ServiceId == request.ServiceId.Value);
            _logger.LogDebug("After ServiceId {id} filter: {complaintCount} complaints", complaints.Count(), request.ServiceId.Value);

        }

        if (request.Status.HasValue)
        {
            complaints = complaints.Where(q => q.ComplaintStatus == request.Status.Value);
            _logger.LogDebug("After Status {Status} filter: {complaintCount} complaints", complaints.Count(), request.Status.Value);

        }

        var complaintList = complaints.ToList();
        
        _logger.LogDebug("Final complaint count {compalintCount}", complaintList.Count);
        
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

        _logger.LogDebug("Complaint statistics - Total: {total}, Pending: {pending}, Reviewed: {reviewed}, Resolved: {resolved}", totalComplaints, pending, reviewed, resolved);

        var response = new ComplaintReportResponseModel
        {
            Complaints = complaintItem,
            TotalComplaints = totalComplaints,
            PendingCount = pending,
            ReviewedCount = reviewed,
            ResolvedCount = resolved
        };
        
        _logger.LogInformation("Complaint report completed.");
        return response;
    }

    public ReviewReportResponseModel GetReviewReport(ReviewReportRequest request)
    {
        _logger.LogInformation("Getting review report with filters - CompanyId: {CompanyId}, EmployeeId: {EmployeeId}, ServiceId: {ServiceId}", request.CompanyId, request.EmployeeId, request.ServiceId);
        
        var reviews = _reportRepository.GetReviewReport(request);
        _logger.LogDebug("{reviewCount} reviews from repository", reviews.Count());
        
        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                _logger.LogError("Invalid date range for review report. From: {From}, To: {To}", request.From, request.To);
                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError("Invalid parameter combination for review report. EmployeeId cannot be used with CompanyId or ServiceId");
                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }

        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError("Invalid parameter combination for review report. CompanyId cannot be used with EmployeeId or ServiceId");
                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }


        if (request.EmployeeId.HasValue)
        {
            var found = _employeeRepository.FindById(request.EmployeeId.Value);

            if (found == null)
            {
                _logger.LogWarning("Employee with Id {id} not found for getting review report.", request.EmployeeId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = _companyRepository.FindById(request.CompanyId.Value);
            if (found == null)
            {
                _logger.LogWarning("Company with Id {id} not found for getting review report.", request.CompanyId.Value);

                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = _serviceRepository.FindById(request.ServiceId.Value);
            if (found == null)
            {
                _logger.LogWarning("Service with Id {id} not found for getting review report.", request.ServiceId.Value);

                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        
        _logger.LogDebug("Applying filter: {reviewCount} reviews", reviews.Count());
        if (request.From.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.StartTime >= request.From.Value.ToUniversalTime());
            _logger.LogDebug("After From filter: {reviewCount} reviews", reviews.Count());
        }

        if (request.To.HasValue)
        {
            reviews = reviews.Where(s =>
                (s.Queue.EndTime.HasValue ? s.Queue.EndTime.Value : s.Queue.StartTime.AddMinutes(30)) <=
                request.To.Value.ToUniversalTime());
            _logger.LogDebug("After To filter: {reviewCount} reviews", reviews.Count());

        }

        if (request.EmployeeId.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.EmployeeId == request.EmployeeId.Value);
            _logger.LogDebug("After EmployeeId {id} filter: {reviewCount} reviews", request.EmployeeId.Value, reviews.Count());

        }

        if (request.CompanyId.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.Service.CompanyId == request.CompanyId.Value);
            _logger.LogDebug("After CompanyId {id} filter: {reviewCount} reviews", request.CompanyId.Value, reviews.Count());

        }

        if (request.ServiceId.HasValue)
        {
            reviews = reviews.Where(s => s.Queue.ServiceId == request.ServiceId.Value);
            _logger.LogDebug("After ServiceId {id} filter: {reviewCount} reviews", request.ServiceId.Value, reviews.Count());

        }


        var reviewList = reviews.ToList();
        _logger.LogDebug("Final review count: {reviewCount}", reviewList.Count());
        
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
            
            _logger.LogDebug("Review statistics - Total: {total}, Average: {average}, Ratings: 1={r1}, 2={r2}, 3={r3}, 4={r4}, 5={r5}", totalReviews, average, rating1, rating2, rating3, rating4, rating5);
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
        
        _logger.LogInformation("Review report completed.");
        return response;
    }

    public ServiceReportResponseModel GetServiceReport(ServiceReportRequest request)
    {
        _logger.LogInformation("Getting service report with filters - CompanyId: {CompanyId}, ServiceId: {ServiceId}", request.CompanyId, request.ServiceId);
        var services = _reportRepository.GetServiceReport(request);
        var totalServices = 0;

        _logger.LogDebug("{serviceCount} services from repository");
        
        if (request.CompanyId.HasValue)
        {
            var found = _companyRepository.FindById(request.CompanyId.Value);
            if (found== null)
            {
                _logger.LogWarning("Company with Id {id} not found for getting service report.", request.CompanyId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = _serviceRepository.FindById(request.ServiceId.Value);
            if (found== null)
            {
                _logger.LogInformation("Service with Id {serviceId} not found for getting service report.", request.ServiceId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        
        if (request.ServiceId.HasValue)
        {
            if (request.CompanyId.HasValue)
            {
                _logger.LogError("Invalid parameter combination for service report. ServiceId cannot be used with CompanyId");
                throw new Exception("ServiceId cannot be used together with CompanyId.");
            }
        }
        
        _logger.LogDebug("Applying filter: {serviceCount} services");

        if (request.CompanyId.HasValue)
        {
            services = services.Where(s => s.CompanyId == request.CompanyId.Value);
            totalServices = services.Count();
            _logger.LogDebug("After CompanyId filter: {serviceCount} services", totalServices);
        }

        if (request.ServiceId.HasValue)
        {
            services = services.Where(s => s.Id == request.ServiceId.Value);
            _logger.LogDebug("After ServiceId filter: {serviceCount} services", services.Count());

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
            
            _logger.LogDebug("Service {ServiceName} queue statistics - Total: {total}, Completed: {completed}, Pending: {pending}, Confirmed: {confirmed}, Cancelled: {cancelled}, DidNotCome: {didNotCome}"
                ,service.ServiceName, totalQueues, completed, pending, confirmed, cancelled,didNotCome );
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
            _logger.LogInformation("Service item report added successfully to service report response model.");
        }

        response.TotalServices = totalServices;
        _logger.LogInformation("Service report completed.");
        return response;
    }
}