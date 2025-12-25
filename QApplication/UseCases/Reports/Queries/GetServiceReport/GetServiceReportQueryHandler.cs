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

namespace QApplication.UseCases.Reports.Queries.GetServiceReport;

public class GetServiceReportQueryHandler: IRequestHandler<GetServiceReportQuery, ServiceReportResponseModel>
{
    private readonly ILogger<GetServiceReportQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetServiceReportQueryHandler(ILogger<GetServiceReportQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ServiceReportResponseModel> Handle(GetServiceReportQuery request, CancellationToken cancellationToken)
    {
         _logger.LogInformation("Getting service report with filters - CompanyId: {CompanyId}, ServiceId: {ServiceId}",
            request.CompanyId, request.ServiceId);
         var services = await _dbContext.GetAllServicesAsync(cancellationToken);
        var totalServices = 0;

        _logger.LogDebug("{serviceCount} services from repository");

        if (request.CompanyId.HasValue)
        {
            
            var found = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.CompanyId.Value,
                cancellationToken);
            if (found == null)
            {
                _logger.LogWarning("Company with Id {id} not found for getting service report.",
                    request.CompanyId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(CompanyEntity));
            }
        }

        if (request.ServiceId.HasValue)
        {
            var found = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == request.ServiceId.Value,
                cancellationToken);
            if (found == null)
            {
                _logger.LogInformation("Service with Id {serviceId} not found for getting service report.",
                    request.ServiceId.Value);
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ServiceEntity));
            }
        }


        if (request.ServiceId.HasValue)
        {
            if (request.CompanyId.HasValue)
            {
                _logger.LogError(
                    "Invalid parameter combination for service report. ServiceId cannot be used with CompanyId");
                throw new Exception("ServiceId cannot be used together with CompanyId.");
            }
        }

        _logger.LogDebug("Applying filter: {serviceCount} services");

        if (request.CompanyId.HasValue)
        {
            services = services.Where(s => s.CompanyId == request.CompanyId.Value).ToList();
            totalServices = services.Count();
            _logger.LogDebug("After CompanyId filter: {serviceCount} services", totalServices);
        }

        if (request.ServiceId.HasValue)
        {
            services = services.Where(s =>  s.Id == request.ServiceId.Value).ToList();
            _logger.LogDebug("After ServiceId filter: {serviceCount} services", services.Count());
        }
        
        var response = new ServiceReportResponseModel();
        foreach (var service in services)
        {
            var queues = await _dbContext.GetAllQueuesAsync(cancellationToken);

            var pending = queues.Count(s => s.Status == QueueStatus.Pending);
            var confirmed = queues.Count(s => s.Status == QueueStatus.Confirmed);
            var completed = queues.Count(s => s.Status == QueueStatus.Completed);
            var cancelledByCustomer = queues.Count(s => s.Status == QueueStatus.CancelledByCustomer);
            var cancelledByEmployee = queues.Count(s => s.Status == QueueStatus.CancelledByEmployee);
            var didNotCome = queues.Count(s => s.Status == QueueStatus.DidNotCome);
            var cancelled = cancelledByCustomer + cancelledByEmployee;
            var totalQueues = pending + confirmed + completed + cancelledByCustomer + cancelledByEmployee + didNotCome;

            _logger.LogDebug(
                "Service {ServiceName} queue statistics - Total: {total}, Completed: {completed}, Pending: {pending}, Confirmed: {confirmed}, Cancelled: {cancelled}, DidNotCome: {didNotCome}"
                , service.ServiceName, totalQueues, completed, pending, confirmed, cancelled, didNotCome);
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