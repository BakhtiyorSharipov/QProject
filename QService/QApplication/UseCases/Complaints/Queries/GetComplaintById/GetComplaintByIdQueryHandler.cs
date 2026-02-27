using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Extensions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Complaints.Queries.GetComplaintById;

public class GetComplaintByIdQueryHandler: IRequestHandler<GetComplaintByIdQuery, ComplaintResponseModel>
{
    private readonly ILogger<GetComplaintByIdQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetComplaintByIdQueryHandler(ILogger<GetComplaintByIdQueryHandler> logger, IQueueApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<ComplaintResponseModel> Handle(GetComplaintByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting complaint by Id {id}", request.Id);

        var isUserEmployee = await _contextAccessor.IsEmployee(_dbContext, cancellationToken);
        int employeeId = 0;
        int customerId = 0;
        if (isUserEmployee)
        {
            var currentEmployee = await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
            employeeId = currentEmployee.CompanyId;
        }
        else
        {
            var currentCustomer = await _contextAccessor.CurrentCustomer(_dbContext, cancellationToken);
            customerId = currentCustomer.Id;
        }
        
        var dbComplaint = await _dbContext.Complaints
            .Where(s=>isUserEmployee
            ? s.Queue.EmployeeId== employeeId
            : s.CustomerId== customerId)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (dbComplaint == null)
        {
            _logger.LogWarning("Complaint with Id {id} not found.", request.Id);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ComplaintEntity));
        }

        var response = new ComplaintResponseModel
        {
            Id = dbComplaint.Id,
            CustomerId = dbComplaint.CustomerId,
            QueueId = dbComplaint.QueueId,
            EmployeeId = dbComplaint.Queue.EmployeeId,
            ComplaintText = dbComplaint.ComplaintText,
            ResponseText = dbComplaint.ResponseText,
            ComplaintStatus = dbComplaint.ComplaintStatus
        };

        _logger.LogInformation("Complaint by Id {id} fetched successfully.", request.Id);
        return response;
    }
}