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

namespace QApplication.UseCases.Reports.Queries.GetQueueReport;

public class GetQueueReportQueryHandler: IRequestHandler<GetQueueReportQuery, QueueReportResponseModel>
{
    private readonly ILogger<GetQueueReportQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetQueueReportQueryHandler(ILogger<GetQueueReportQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<QueueReportResponseModel> Handle(GetQueueReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting queue report with filters -CompanyId: {companyId}, EmployeeId: {employeeId}, ServiceId: {serviceId}, Status: {status}"
            , request.CompanyId, request.EmployeeId, request.ServiceId, request.Status);
        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                _logger.LogError("Invalid date range for queue report. From: {From}, To: {To}", request.From,
                    request.To);
                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError(
                    "Invalid parameter combination for queue report. EmployeeId cannot be used with CompanyId or ServiceId");
                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }

        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError(
                    "Invalid parameter combination for queue report. CompanyId cannot be used with EmployeeId or ServiceId");
                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            var found = await _dbContext.Employees.FirstOrDefaultAsync(s => s.Id == request.EmployeeId.Value,
                cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Employee with Id {id} not found for getting queue report.",
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
                _logger.LogWarning("Company with Id {id} not found for getting queue report.", request.CompanyId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == request.ServiceId.Value,
                cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Service with Id {id} not found for getting queue report.", request.ServiceId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        var queues = await _dbContext.GetAllQueuesAsync(cancellationToken);
        _logger.LogDebug("{queueCount} queues from repository", queues.Count());


        if (request.From.HasValue)
        {
            queues = queues.Where(q => q.StartTime >= request.From.Value.ToUniversalTime()).ToList();
            _logger.LogDebug("After From filter: {queueCount} queues", queues.Count());
        }

        if (request.To.HasValue)
        {
            queues = queues.Where(q =>
                (q.EndTime.HasValue ? q.EndTime.Value : q.StartTime.AddMinutes(30)) <=
                request.To.Value.ToUniversalTime()).ToList();
            _logger.LogDebug("After To filter: {queueCount} queues", queues.Count());
        }


        if (request.EmployeeId.HasValue)
        {
            queues = queues.Where(q => q.Service != null && q.EmployeeId == request.EmployeeId.Value).ToList();
            _logger.LogDebug("After EmployeeId {id} filter: {queueCount} queues", request.EmployeeId.Value,
                queues.Count());
        }

        if (request.ServiceId.HasValue)
        {
            queues = queues.Where(q => q.Service != null && q.ServiceId == request.ServiceId.Value).ToList();
            _logger.LogDebug("After ServiceId {id} filter: {queueCount} queues", request.ServiceId.Value,
                queues.Count());
        }

        if (request.CompanyId.HasValue)
        {
            queues = queues.Where(q => q.Service != null && q.Service.CompanyId == request.CompanyId.Value).ToList();
            _logger.LogDebug("After CompanyId {id} filter: {queueCount} queues", request.CompanyId.Value,
                queues.Count());
        }

        if (request.Status.HasValue)
        {
            queues = queues.Where(q => q.Status == request.Status.Value).ToList();
            _logger.LogDebug("After Status {Status} filter: {queueCount} queues", request.Status.Value, queues.Count());
        }

        var queueList = queues.ToList();
        _logger.LogDebug("Final queue count {queueCount}", queueList.Count);

        var queueItem = queueList.Select(queue => new QueueReportItemResponseModel
        {
            Id = queue.Id,
            EmployeeName = queue.Employee?.FirstName ?? "Unknown Employee",
            CompanyName = queue.Service?.Company?.CompanyName ?? "Unknown Company",
            ServiceName = queue.Service?.ServiceName ?? "Unknown Service",
            CustomerName = queue.Customer?.FirstName ?? "Unknown Customer",
            StartTime = queue.StartTime,
            EndTime = queue.EndTime ?? queue.StartTime.AddMinutes(30),
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

        _logger.LogDebug(
            "Queue statistics - Total: {totalQueues}, Completed: {compelted}, Pending: {pending}, Confirmed: {confirmed}, Cancelled: {cancelled}, DidNotCome: {didNotCome}"
            , totalQueues, completedCount, pendingCount, confirmedCount, canceledCount, didNotComeCount);

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
}