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

   
}