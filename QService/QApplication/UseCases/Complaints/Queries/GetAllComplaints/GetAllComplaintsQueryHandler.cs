using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Extensions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Complaints.Queries.GetAllComplaints;

public class
    GetAllComplaintsQueryHandler : IRequestHandler<GetAllComplaintsQuery, PagedResponse<ComplaintResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetAllComplaintsQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetAllComplaintsQueryHandler(ILogger<GetAllComplaintsQueryHandler> logger,
        IQueueApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<PagedResponse<ComplaintResponseModel>> Handle(GetAllComplaintsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all complaints. PageNumber: {pageNumber}, PageSize: {pageSize}",
            request.PageNumber,
            PageSize);


        var isUserEmployee = await _contextAccessor.IsEmployee(_dbContext, cancellationToken);
        int employeeId = 0;
        int customerId = 0;
        if (isUserEmployee)
        {
            var currentEmployee = await _contextAccessor.CurrentEmployee(_dbContext, cancellationToken);
            employeeId = currentEmployee.Id;
        }
        else
        {
            var currentCustomer = await _contextAccessor.CurrentCustomer(_dbContext, cancellationToken);
            customerId = currentCustomer.Id;
        }


        var totalCount = await _dbContext.Complaints
            .Where(s => isUserEmployee
                ? s.Queue.EmployeeId == employeeId
                : s.CustomerId == customerId)
            .CountAsync(cancellationToken);

        var dbComplaints = await _dbContext.Complaints
            .Include(s => s.Queue)
            .Where(s => isUserEmployee
                ? s.Queue.EmployeeId == employeeId
                : s.CustomerId == customerId)
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);


        var response = dbComplaints.Select(complaint => new ComplaintResponseModel()
        {
            Id = complaint.Id,
            CustomerId = complaint.CustomerId,
            QueueId = complaint.QueueId,
            EmployeeId = complaint.Queue.EmployeeId,
            ComplaintText = complaint.ComplaintText,
            ResponseText = complaint.ResponseText,
            ComplaintStatus = complaint.ComplaintStatus
        }).ToList();

        _logger.LogInformation("Fetched {complaintsCount} complaints.", response.Count);

        return new PagedResponse<ComplaintResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}