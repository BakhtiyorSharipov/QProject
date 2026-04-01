using System.Net;
using Microsoft.Extensions.Logging;
using QAggregationService.Application.Exceptions;
using QAggregationService.Contracts.Interfaces;
using QAggregationService.Contracts.Requests;
using QAggregationService.Contracts.Responses;
using QBranchService.Contracts.Interfaces;
using QBranchService.Contracts.Requests;
using QContracts.Enums;
using QContracts.Interfaces;
using QContracts.Responses;

namespace QAggregationService.Application.Services;

public class AggregationService : IAggregationService
{
    private readonly IQueueService _queueService;
    private readonly IBranchService _branchService;
    private readonly ILogger<AggregationService> _logger;

    public AggregationService(IQueueService queueService, IBranchService branchService,
        ILogger<AggregationService> logger)
    {
        _queueService = queueService;
        _branchService = branchService;
        _logger = logger;
    }

    public async Task<CompanyReportResponse> GetReportAsync(ReportRequest request)
    {
        if (!request.CompanyId.HasValue)
        {
            throw new ArgumentException("Company is required");
        }

        _logger.LogInformation("Fetching report for company Id {companyId}", request.CompanyId.Value);

        var companyResult = await _branchService.CheckCompanyId(new CompanyRequest
        {
            RequestId = Guid.NewGuid(),
            CompanyId = request.CompanyId.Value,
            RequestedAt = DateTimeOffset.UtcNow
        });

        if (!companyResult.IsValid)
        {
            _logger.LogInformation("Company with Id {CompanyId} not found", request.CompanyId.Value);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                companyResult.ErrorMessage ?? "Company not found");
        }


        string branchName = null;
        if (request.BranchId.HasValue)
        {
            var branchResult = await _branchService.CheckBranchId(new BranchRequest
            {
                RequestId = Guid.NewGuid(),
                CompanyId = request.CompanyId.Value,
                BranchId = request.BranchId.Value,
                RequestedAt = DateTimeOffset.UtcNow
            });

            if (!branchResult.IsValid)
            {
                _logger.LogInformation("Branch with Id {branchId} not found", request.BranchId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    companyResult.ErrorMessage ?? "Branch not found");
            }

            if (branchResult.BranchName != null)
            {
                branchName = branchResult.BranchName;
            }
        }

        string serviceName = null;
        if (request.ServiceId.HasValue)
        {
            var companyServiceResult = await _branchService.CheckCompanyServiceId(new CompanyServiceRequest
            {
                RequestId = Guid.NewGuid(),
                CompanyId = request.CompanyId.Value,
                CompanyServiceId = request.ServiceId.Value,
                RequestedAt = DateTimeOffset.UtcNow
            });

            if (!companyServiceResult.IsValid)
            {
                _logger.LogInformation("Company service with Id {serviceId} not found", request.ServiceId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    companyResult.ErrorMessage ?? "Company service not found");
            }

            if (companyServiceResult.CompanyServiceName != null)
            {
                serviceName = companyServiceResult.CompanyServiceName;
            }
        }

        var companyQueues = await _queueService.GetCompanyQueuesAsync(request.CompanyId.Value);
        var companyReviews = await _queueService.GetCompanyReviewsAsync(request.CompanyId.Value);
        var companyComplaints = await _queueService.GetCompanyComplaintsAsync(request.CompanyId.Value);
        var customers = await _queueService.GetAllCompanyCustomers(request.CompanyId.Value);
        var blockedCustomers = await _queueService.GetAllCompanyBlockedCustomers(request.CompanyId.Value);
        var employees = await _queueService.GetAllCompanyEmployees(request.CompanyId.Value);

        var filteredQueues = companyQueues.AsEnumerable();
        if (request.BranchId.HasValue)
        {
            filteredQueues = filteredQueues.Where(s => s.BranchId == request.BranchId.Value);
        }

        if (request.ServiceId.HasValue)
        {
            filteredQueues = filteredQueues.Where(s => s.ServiceId == request.ServiceId.Value);
        }

        if (request.FromDate.HasValue)
            filteredQueues = filteredQueues.Where(s => s.StartTime >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            filteredQueues =
                filteredQueues.Where(s => (s.EndTime ?? s.StartTime.AddMinutes(30)) <= request.ToDate.Value);

        if (request.QueueStatus.HasValue)
            filteredQueues = filteredQueues.Where(s => s.CurrentQueueStatus == request.QueueStatus.Value);

        var filteredQueuesList = filteredQueues.ToList();
        var totalRecords = filteredQueuesList.Count;

        var pagedQueues = filteredQueuesList
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();


        var filteredQueuesId = filteredQueuesList.Select(q => q.Id).ToList();

        var filteredReviews = companyReviews
            .Where(r => filteredQueuesId.Contains(r.QueueId))
            .ToList();

        var filteringComplaints = companyComplaints
            .Where(c => filteredQueuesId.Contains(c.QueueId))
            .AsEnumerable();

        if (request.ComplaintStatus.HasValue)
        {
            filteringComplaints = filteringComplaints.Where(s => s.Status == request.ComplaintStatus);
        }

        var filteredComplaints = filteringComplaints.ToList();

        var totalQueues = filteredQueuesId.Count;
        var completedQueues = filteredQueuesList.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.Completed);
        var pendingQueues = filteredQueuesList.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.Pending);
        var confirmedQueues = filteredQueuesList.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.Confirmed);
        var cancelledQueues = filteredQueuesList.Count(s =>
            s.CurrentQueueStatus == CurrentQueueStatus.CancelledByCustomer
            || s.CurrentQueueStatus == CurrentQueueStatus.CancelledByEmployee
            || s.CurrentQueueStatus == CurrentQueueStatus.CanceledByAdmin);
        var didNotCome = filteredQueuesList.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.DidNotCome);

        var averageRating = filteredReviews.Any() ? filteredReviews.Average(r => r.Grade) : 0;
        var totalReviews = filteredReviews.Count;
        var fiveStarReviews = filteredReviews.Count(s => s.Grade == 5);
        var fourStarReviews = filteredReviews.Count(s => s.Grade == 4);
        var threeStarReviews = filteredReviews.Count(s => s.Grade == 3);
        var twoStarReviews = filteredReviews.Count(s => s.Grade == 2);
        var oneStarReviews = filteredReviews.Count(s => s.Grade == 1);

        var totalComplaints = filteredComplaints.Count;
        var pendingComplaints = filteredComplaints.Count(s => s.Status == CurrentComplaintStatus.Pending);
        var reviewedComplaints = filteredComplaints.Count(s => s.Status == CurrentComplaintStatus.Reviewed);
        var resolvedComplaints = filteredComplaints.Count(s => s.Status == CurrentComplaintStatus.Resolved);

        var totalCustomers = customers.Count;
        var totalEmployees = employees.Count;
        var totalBlockedCustomers = blockedCustomers.Count;

        var response = new CompanyReportResponse
        {
            CompanyId = request.CompanyId.Value,
            CompanyName = companyResult.CompanyName ?? "Unknown",
            ReportDate = DateTime.UtcNow,
            TotalQueues = totalQueues,
            CompletedQueues = completedQueues,
            PendingQueues = pendingQueues,
            ConfirmedQueues = confirmedQueues,
            CancelledQueues = cancelledQueues,
            DidNotComeQueues = didNotCome,
            AverageRating = averageRating,
            TotalReviews = totalReviews,
            FiveStarReviews = fiveStarReviews,
            FourStarReviews = fourStarReviews,
            ThreeStarReviews = threeStarReviews,
            TwoStarReviews = twoStarReviews,
            OneStarReviews = oneStarReviews,
            TotalComplaints = totalComplaints,
            PendingComplaints = pendingComplaints,
            ReviewedComplaints = reviewedComplaints,
            ResolvedComplaints = resolvedComplaints,
            TotalEmployees = totalEmployees,
            TotalCustomers = totalCustomers,
            TotalBlockedCustomers = totalBlockedCustomers,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize)
        };

        response.Queues = pagedQueues.Select(q => new QueueReportItem
        {
            Id = q.Id,
            CustomerName = q.CustomerName,
            EmployeeName = q.EmployeeName,
            Status = q.CurrentQueueStatus.ToString(),
            StartTime = q.StartTime,
            EndTime = q.EndTime
        }).ToList();

        return response;
    }

    public async Task<DashboardResponse> GetCompanyDashboard(int companyId)
    {
        _logger.LogInformation("Fetching dashboard for company Id {companyId}", companyId);

        var companyResult = await _branchService.CheckCompanyId(new CompanyRequest
        {
            RequestId = Guid.NewGuid(),
            CompanyId = companyId,
            RequestedAt = DateTimeOffset.UtcNow
        });

        if (!companyResult.IsValid)
        {
            _logger.LogInformation("Company with Id {CompanyId} not found", companyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                companyResult.ErrorMessage ?? "Company not found");
        }

        var companyBranches = await _branchService.GetCompanyBranches(companyId);
        var companyServices = await _branchService.GetCompanyServices(companyId);
        var companyCustomers = await _queueService.GetAllCompanyCustomers(companyId);
        var companyBlockedCustomers = await _queueService.GetAllCompanyBlockedCustomers(companyId);
        var companyEmployees = await _queueService.GetAllCompanyEmployees(companyId);
        var companyQueues = await _queueService.GetCompanyQueuesAsync(companyId);
        var companyReviews = await _queueService.GetCompanyReviewsAsync(companyId);
        var companyComplaints = await _queueService.GetCompanyComplaintsAsync(companyId);

        var totalBranches = companyBranches.Count;
        var totalServices = companyServices.Count;
        var totalCustomers = companyCustomers.Count;
        var totalBlockedCustomers = companyBlockedCustomers.Count;
        var totalEmployees = companyEmployees.Count;
        var totalQueues = companyQueues.Count;
        var totalCompletedQueues = companyQueues.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.Completed);
        var totalPendingQueues = companyQueues.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.Pending);
        var totalCancelledQueues = companyQueues.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.CanceledByAdmin
                                                            || s.CurrentQueueStatus ==
                                                            CurrentQueueStatus.CancelledByCustomer
                                                            || s.CurrentQueueStatus ==
                                                            CurrentQueueStatus.CancelledByEmployee);

        var totalDidNotComeQueues = companyQueues.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.DidNotCome);
        var totalReviews = companyReviews.Count;
        var totalComplaints = companyComplaints.Count;
        var totalPendingComplaints = companyComplaints.Count(s => s.Status == CurrentComplaintStatus.Pending);
        var totalReviewedComplaints = companyComplaints.Count(s => s.Status == CurrentComplaintStatus.Reviewed);
        var totalResolvedComplaints = companyComplaints.Count(s => s.Status == CurrentComplaintStatus.Resolved);

        var groupedByService = companyQueues
            .GroupBy(s => s.ServiceId)
            .OrderByDescending(s => s.Count())
            .FirstOrDefault();


        int? topServiceId = groupedByService?.Key;
        int topServiceQueueCount = groupedByService?.Count() ?? 0;
        string topServiceName = "Unknown";
        if (topServiceId.HasValue)
        {
            var topService = companyServices.FirstOrDefault(s => s.CompanyServiceId == topServiceId);
            if (topService != null)
            {
                topServiceName = topService.CompanyServiceName;
            }
        }

        var groupedByBranch = companyQueues
            .GroupBy(s => s.BranchId)
            .OrderByDescending(s => s.Count())
            .FirstOrDefault();

        int? topBranchId = groupedByBranch?.Key;
        int topBranchQueueCount = groupedByBranch?.Count() ?? 0;
        string topBranchName = "Unknown";
        if (topBranchId.HasValue)
        {
            var topBranch = companyBranches.FirstOrDefault(s => s.BranchId == topBranchId);
            if (topBranch != null)
            {
                topBranchName = topBranch.BranchName;
            }
        }


        var groupedByEmployee = companyQueues
            .GroupBy(s => s.EmployeeId)
            .OrderByDescending(s => s.Count())
            .FirstOrDefault();

        int? topEmployeeId = groupedByEmployee?.Key;
        int topEmployeeQueueCount = groupedByEmployee?.Count() ?? 0;
        string topEmployeeName = "Unknown";
        if (topEmployeeId.HasValue)
        {
            var topEmployee = companyEmployees.FirstOrDefault(s => s.EmployeeId == topEmployeeId);
            if (topEmployee != null)
            {
                topEmployeeName = topEmployee.FirstName;
            }
        }


        var groupedByCustomer = companyQueues
            .GroupBy(s => s.CustomerId)
            .OrderByDescending(s => s.Count())
            .FirstOrDefault();

        int? topCustomerId = groupedByCustomer?.Key;
        int topCustomerQueueCount = groupedByCustomer?.Count() ?? 0;
        string topCustomerName = "Unknown";
        if (topCustomerId.HasValue)
        {
            var topCustomer = companyCustomers.FirstOrDefault(s => s.CustomerId == topCustomerId);
            if (topCustomer != null)
            {
                topCustomerName = topCustomer.FirstName;
            }
        }

        var recentQueues = companyQueues
            .OrderByDescending(q => q.CreatedAt)
            .Take(5)
            .ToList();

        var recentQueueItems = recentQueues.Select(q => new QueueReportItem
        {
            Id = q.Id,
            CustomerName = q.CustomerName,
            EmployeeName = q.EmployeeName,
            Status = q.CurrentQueueStatus.ToString(),
            StartTime = q.StartTime,
            EndTime = q.EndTime
        }).ToList();

        var response = new DashboardResponse
        {
            CompanyId = companyId,
            CompanyName = companyResult.CompanyName ?? "Unknown",
            ReportDate = DateTime.UtcNow,
            TotalBranches = totalBranches,
            TotalServices = totalServices,
            TotalCustomers = totalCustomers,
            TotalBlockedCustomers = totalBlockedCustomers,
            TotalEmployees = totalEmployees,
            TotalQueues = totalQueues,
            CompletedQueues = totalCompletedQueues,
            PendingQueues = totalPendingQueues,
            CancelledQueues = totalCancelledQueues,
            DidNotComeQueues = totalDidNotComeQueues,
            TotalReviews = totalReviews,
            TotalComplaints = totalComplaints,
            TotalPendingComplaints = totalPendingComplaints,
            TotalReviewedComplaints = totalReviewedComplaints,
            TotalResolvedComplaints = totalResolvedComplaints,
            TopServiceName = topServiceName,
            TopServiceQueueCount = topServiceQueueCount,
            TopBranchName = topBranchName,
            TopBranchQueueCount = topBranchQueueCount,
            TopEmployeeName = topEmployeeName,
            TopEmployeeQueueCount = topEmployeeQueueCount,
            TopCustomerName = topCustomerName,
            TopCustomerQueueCount = topCustomerQueueCount,
            RecentQueues = recentQueueItems
        };

        return response;
    }

    public async Task<EmployeeReportResponse> GetEmployeeReport(EmployeeReportRequest request)
    {
        var employees = await _queueService.GetAllEmployees();
        if (!employees.Any())
        {
            _logger.LogWarning("Not found any employee");
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Not found any employee");
        }

        var employeeId = employees.FirstOrDefault(s => s.EmployeeId == request.EmployeeId);
        if (employeeId == null)
        {
            _logger.LogWarning("Not found employee with Id {employeeId}", request.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"Not found employee with Id {request.EmployeeId}");
        }

        var totalEmployeeQueues = await _queueService.GetEmployeeQueuesAsync(request.EmployeeId);
        var totalEmployeeReviews = await _queueService.GetEmployeeReviewsAsync(request.EmployeeId);
        var totalEmployeeComplaints = await _queueService.GetEmployeeComplaintsAsync(request.EmployeeId);

        var filteredQueues = totalEmployeeQueues.AsEnumerable();
        var filteredReviews = totalEmployeeReviews.AsEnumerable();
        var filteredComplaints = totalEmployeeComplaints.AsEnumerable();
        if (request.FromDate.HasValue)
        {
            filteredQueues = filteredQueues.Where(s => s.StartTime >= request.FromDate.Value);
            filteredReviews = filteredReviews.Where(s => s.CreatedAt >= request.FromDate.Value);
            filteredComplaints = filteredComplaints.Where(s => s.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            filteredQueues =
                filteredQueues.Where(s => (s.EndTime ?? s.StartTime.AddMinutes(30)) <= request.ToDate.Value);
            filteredReviews = filteredReviews.Where(s => s.CreatedAt <= request.ToDate.Value);
            filteredComplaints = filteredComplaints.Where(s => s.CreatedAt <= request.ToDate.Value);
        }

        var filteredQueuesList = filteredQueues.ToList();
        var filteredReviewList = filteredReviews.ToList();
        var filteredComplaintList = filteredComplaints.ToList();
        var totalQueues = filteredQueuesList.Count;
        var completedQueues = filteredQueuesList.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.Completed);
        var pendingQueues = filteredQueuesList.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.Pending);
        var cancelledQueues = filteredQueuesList.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.CanceledByAdmin
                                                            || s.CurrentQueueStatus ==
                                                            CurrentQueueStatus.CancelledByCustomer
                                                            || s.CurrentQueueStatus ==
                                                            CurrentQueueStatus.CancelledByEmployee);
        var didNotComeQueues = filteredQueuesList.Count(s => s.CurrentQueueStatus == CurrentQueueStatus.DidNotCome);

        var averageEmployeeReviewGrade = filteredReviewList.Average(s => s.Grade);
        var totalReviews = filteredReviewList.Count();
        var totalComplaints = filteredComplaintList.Count();
        var pendingComplaints = filteredComplaintList.Count(s => s.Status == CurrentComplaintStatus.Pending);
        var reviewedComplaints = filteredComplaintList.Count(s => s.Status == CurrentComplaintStatus.Reviewed);
        var resolvedComplaints = filteredComplaintList.Count(s => s.Status == CurrentComplaintStatus.Resolved);

        var recentQueues = filteredQueuesList
            .OrderByDescending(s => s.CreatedAt)
            .Take(5)
            .ToList();

        var recentQueueItem = recentQueues.Select(q => new QueueReportItem()
        {
            Id = q.Id,
            CustomerName = q.CustomerName,
            EmployeeName = q.EmployeeName,
            Status = q.CurrentQueueStatus.ToString(),
            StartTime = q.StartTime,
            EndTime = q.EndTime
        }).ToList();

        var response = new EmployeeReportResponse
        {
            EmployeeId = employeeId.EmployeeId,
            EmployeeName = employeeId.FirstName,
            TotalQueues = totalQueues,
            CompletedQueues = completedQueues,
            PendingQueues = pendingQueues,
            CancelledQueues = cancelledQueues,
            DidNotComeQueues = didNotComeQueues,
            AverageReviewGrade = averageEmployeeReviewGrade,
            TotalReviewsReceived = totalReviews,
            TotalComplaintsReceived = totalComplaints,
            PendingComplaints = pendingComplaints,
            ReviewedComplaints = reviewedComplaints,
            ResolvedComplaints = resolvedComplaints,
            RecentQueues = recentQueueItem
        };

        return response;
    }

    public Task<CustomerReportResponse> GetCustomerReport(CustomerReportRequest request)
    {
        throw new NotImplementedException();
    }
}