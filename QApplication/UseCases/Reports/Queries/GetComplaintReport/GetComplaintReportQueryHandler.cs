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

namespace QApplication.UseCases.Reports.Queries.GetComplaintReport;

public class GetComplaintReportQueryHandler: IRequestHandler<GetComplaintReportQuery, ComplaintReportResponseModel>
{
    private readonly ILogger<GetComplaintReportQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetComplaintReportQueryHandler(ILogger<GetComplaintReportQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ComplaintReportResponseModel> Handle(GetComplaintReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting complaint report with filters - CompanyId: {CompanyId}, EmployeeId: {EmployeeId}, ServiceId: {ServiceId}, Status: {Status}.",
            request.CompanyId, request.EmployeeId, request.ServiceId, request.Status);
        var complaints = await _dbContext.GetAllComplaintsAsync(cancellationToken);
        _logger.LogDebug("{complaintCount} complaints from repository", complaints.Count());

        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                _logger.LogError("Invalid date range for complaint report. From: {From}, To: {To}", request.From,
                    request.To);

                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError(
                    "Invalid parameter combination for complaint report. EmployeeId cannot be used with CompanyId or ServiceId");

                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }

        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError(
                    "Invalid parameter combination for complaint report. CompanyId cannot be used with EmployeeId or ServiceId");

                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            var found = await _dbContext.Employees.FirstOrDefaultAsync(s => s.Id == request.EmployeeId.Value,
                cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Employee with Id {id} not found for getting complaint report.",
                    request.EmployeeId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.CompanyId.Value,
                cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Company with Id {id} not found for getting complaint report.",
                    request.CompanyId.Value);

                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == request.ServiceId,
                cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Service with Id {id} not found for getting complaint report.",
                    request.ServiceId.Value);
                
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        _logger.LogDebug("Applying filter: {compalintCount} complaints.", complaints.Count());
        if (request.From.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.StartTime >= request.From.Value).ToList();
            _logger.LogDebug("After From filter: {complaintCount} complaints", complaints.Count());
        }

        if (request.To.HasValue)
        {
            complaints = complaints.Where(c =>
                (c.Queue.EndTime.HasValue ? c.Queue.EndTime.Value : c.Queue.StartTime.AddMinutes(30)) <=
                request.To.Value).ToList();
            _logger.LogDebug("After To filter: {complaintCount} complaints", complaints.Count());
        }

        if (request.EmployeeId.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.EmployeeId == request.EmployeeId.Value).ToList();
            _logger.LogDebug("After EmployeeId {id} filter: {complaintCount} complaints", complaints.Count(),
                request.EmployeeId.Value);
        }

        if (request.CompanyId.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.Service != null && c.Queue.Service.CompanyId == request.CompanyId.Value).ToList();
            _logger.LogDebug("After CompanyId {id} filter: {complaintCount} complaints", complaints.Count(),
                request.CompanyId.Value);
        }

        if (request.ServiceId.HasValue)
        {
            complaints = complaints.Where(c => c.Queue.ServiceId == request.ServiceId.Value).ToList();
            _logger.LogDebug("After ServiceId {id} filter: {complaintCount} complaints", complaints.Count(),
                request.ServiceId.Value);
        }

        if (request.Status.HasValue)
        {
            complaints = complaints.Where(q => q.ComplaintStatus == request.Status.Value).ToList();
            _logger.LogDebug("After Status {Status} filter: {complaintCount} complaints", complaints.Count(),
                request.Status.Value);
        }

      

        _logger.LogDebug("Final complaint count {complaintCount}", complaints.Count);

        var complaintItem = complaints.Select(complaint => new ComplaintReportItemResponseModel
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

        _logger.LogDebug(
            "Complaint statistics - Total: {total}, Pending: {pending}, Reviewed: {reviewed}, Resolved: {resolved}",
            totalComplaints, pending, reviewed, resolved);

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
}