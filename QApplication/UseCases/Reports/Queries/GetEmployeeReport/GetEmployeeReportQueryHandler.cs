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

namespace QApplication.UseCases.Reports.Queries.GetEmployeeReport;

public class GetEmployeeReportQueryHandler: IRequestHandler<GetEmployeeReportQuery, EmployeeReportResponseModel>
{
    private readonly ILogger<GetEmployeeReportQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetEmployeeReportQueryHandler(ILogger<GetEmployeeReportQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<EmployeeReportResponseModel> Handle(GetEmployeeReportQuery request, CancellationToken cancellationToken)
    {
         _logger.LogInformation(
            "Starting employee report with filters - EmployeeId: {EmployeeId}, CompanyId: {CompanyId}, ServiceId: {ServiceId}",
            request.EmployeeId, request.CompanyId, request.ServiceId);
        var employees = await _dbContext.GetAllEmployeesAsync(cancellationToken);

        var totalEmployees = employees.Count();

        _logger.LogDebug("Employee count: {employeeCount}", totalEmployees);

        if (request.EmployeeId.HasValue)
        {
            var found = await _dbContext.Employees.FirstOrDefaultAsync(s => s.Id == request.EmployeeId.Value,
                cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Employee with Id {id} not found for getting report.", request.EmployeeId);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(EmployeeEntity));
            }
        }

        if (request.CompanyId.HasValue)
        {
            var found = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.CompanyId.Value, cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Company with Id {id} not found for getting employee report.", request.CompanyId);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == request.ServiceId.Value,
                cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Service with Id {id} not found for getting employee report.", request.ServiceId);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }

        if (request.To.HasValue)
        {
            if (request.From.HasValue && request.From.Value > request.To.Value)
            {
                _logger.LogError("Invalid date range for employee report. From: {From}, To: {To}", request.From,
                    request.To);
                throw new Exception("'From' must be less than 'To'");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.CompanyId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError(
                    "Invalid parameter combination for employee report. EmployeeId cannot be used with CompanyId or ServiceId");
                throw new Exception("EmployeeId cannot be used together with CompanyId or ServiceId.");
            }
        }

        if (request.CompanyId.HasValue)
        {
            if (request.EmployeeId.HasValue || request.ServiceId.HasValue)
            {
                _logger.LogError(
                    "Invalid parameter combination for employee report. CompanyId cannot be used with EmployeeId or ServiceId");
                throw new Exception("CompanyId cannot be used together with EmployeeId or ServiceId.");
            }
        }

        if (request.EmployeeId.HasValue)
        {
            _logger.LogDebug("Filtering by EmployeeId {id}", request.EmployeeId.Value);
            employees = employees.Where(s => s.Id == request.EmployeeId.Value).ToList();
        }

        if (request.CompanyId.HasValue)
        {
            _logger.LogDebug("Filtering by CompanyId {id}", request.CompanyId.Value);
            employees = employees
                .Where(s => s.Service != null && s.Service.CompanyId == request.CompanyId.Value)
                .ToList();
            // employees = employees.Where(s => s.Service.CompanyId == request.CompanyId.Value).ToList();
        }

        if (request.ServiceId.HasValue)
        {
            _logger.LogDebug("Filtering by ServiceId {id}", request.ServiceId.Value);
            employees = employees.Where(s => s.ServiceId == request.ServiceId.Value).ToList();
        }


        var employeeList = employees.ToList();
        _logger.LogDebug("After filtering employees {employeeCount}", employeeList.Count);

        var response = new EmployeeReportResponseModel();
        foreach (var employee in employeeList)
        {
            var queues = await _dbContext.Queues.Where(s => s.EmployeeId == employee.Id).ToListAsync(cancellationToken);
            
            if (request.From.HasValue)
            {
                queues = queues.Where(s => s.StartTime >= request.From.Value.ToUniversalTime()).ToList();
                _logger.LogDebug("After From filter for EmployeeId {id}: {queueCount} queues.", employee.Id,
                    queues.Count());
            }

            if (request.To.HasValue)
            {
                queues = queues.Where(s =>
                    (s.EndTime.HasValue ? s.EndTime.Value : s.StartTime.AddMinutes(30)) <=
                    request.To.Value.ToUniversalTime()).ToList();
                _logger.LogDebug("After To filter for EmployeeId {id}: {queueCount} queues.", employee.Id,
                    queues.Count());
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
                CancelledQueues = canceledByCustomer + canceledByEmployee,
                DidNotComeQueues = didNotCome,
            };

            response.Employees.Add(employeeItem);
            _logger.LogInformation("Employee report item successfully added to employee report response model.");
        }

        response.TotalEmployees = totalEmployees;
        _logger.LogInformation("Employee report completed.");
        return response;
    }
}