using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses.ReportResponse;
using QApplication.UseCases.Reports.ReportQueryExtensions;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.Reports.Queries;

public class GetCompanyReportQueryHandler: IRequestHandler<GetCompanyReportQuery, CompanyReportItemResponseModel>
{
    private readonly ILogger<GetCompanyReportQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetCompanyReportQueryHandler(ILogger<GetCompanyReportQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<CompanyReportItemResponseModel> Handle(GetCompanyReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting company report for CompanyId: {id} with date range: {from} to {to}",
            request.CompanyId, request.From, request.To);
        var company = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.CompanyId, cancellationToken);
        if (company == null)
        {
            _logger.LogWarning("Company with Id {id} not found for getting company report.", request.CompanyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
        }

        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                _logger.LogError(
                    "Invalid date range for company report. CompanyId: {CompanyId}, From: {From}, To: {To}",
                    request.CompanyId, request.From, request.To);
                throw new Exception("'From' must be less than 'To'");
            }
        }

        _logger.LogDebug("Fetching data for company with Id {id} report.", request.CompanyId);

        var queues = await _dbContext.GetQueuesByCompanyIdAsync(request.CompanyId, cancellationToken);


        var employees = await _dbContext.GetEmployeesByCompanyIdAsync(request.CompanyId, cancellationToken);

        var services = await _dbContext.GetServicesByCompanyIdAsync(request.CompanyId, cancellationToken);

        var customers = await _dbContext.GetCustomersByCompanyIdAsync(request.CompanyId, cancellationToken);

        var reviews = await _dbContext.GetReviewsByCompanyIdAsync(request.CompanyId, cancellationToken);

        var complaints = await _dbContext.GetComplaintsByCompanyIdAsync(request.CompanyId, cancellationToken);

        var blockedCustomers =
            await _dbContext.GetBlockedCustomersByCompanyIdASync(request.CompanyId, cancellationToken);

        _logger.LogDebug("Base data - Queues: {queueCount}, Employees: {employeeCount}, Services: {serviceCount}",
            queues.Count(), employees.Count(), services.Count());
        if (request.From.HasValue)
        {
            _logger.LogDebug("Data filtering From: {from}, Current queues: {queues}",
                request.From.Value.ToUniversalTime(), queues.Count());
            queues = queues.Where(s => s.StartTime >= request.From.Value.ToUniversalTime()).ToList();
            _logger.LogDebug("After From filter: {queues} queues", queues.Count());
        }


        if (request.To.HasValue)
        {
            _logger.LogDebug("Data filtering To: {to}, Current queues: {queues}", request.To.Value.ToUniversalTime(),
                queues.Count());
            queues = queues.Where(s =>
                (s.EndTime.HasValue ? s.EndTime.Value : s.StartTime.AddMinutes(30)) <=
                request.To.Value.ToUniversalTime()).ToList();
            _logger.LogDebug("After To filter: {queues} queues", queues.Count());
        }

        var totalQueues = queues.Count();
        var completedQueues = queues.Count(s => s.Status == QueueStatus.Completed);
        var cancelledQueues = queues.Count(q =>
            q.Status == QueueStatus.CancelledByCustomer || q.Status == QueueStatus.CancelledByEmployee);
        var didNotComeQueues = queues.Count(s => s.Status == QueueStatus.DidNotCome);

        _logger.LogDebug(
            "Queue statistics - Total: {totalQueues}, Completed: {Completed}, Cancelled: {cancelled}, DidNotCome: {didNotCome}",
            totalQueues, completedQueues, cancelledQueues, didNotComeQueues);

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
        
        
        _logger.LogDebug(
            "Calculated - AverageRating: {avRa}, Complaints: {complaints}, PopularServices: {popularServices}",
            averageRating, complaintCount, mostPopularServices.Count);

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
}