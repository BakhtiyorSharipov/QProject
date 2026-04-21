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

namespace QApplication.UseCases.Complaints.Queries.GetAllCustomerComplints;

public class GetComplaintsByCustomerQueryHandler: IRequestHandler<GetComplaintsByCustomerQuery, PagedResponse<ComplaintResponseModel>>
{
    private const int PageSize = 15;
    private readonly ILogger<GetComplaintsByCustomerQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetComplaintsByCustomerQueryHandler(ILogger<GetComplaintsByCustomerQueryHandler> logger, IQueueApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<PagedResponse<ComplaintResponseModel>> Handle(GetComplaintsByCustomerQuery request, CancellationToken cancellationToken)
    {
        var currentCustomer = await _contextAccessor.CurrentCustomer(_dbContext, cancellationToken);
        var customerId = currentCustomer.Id;
        

        _logger.LogInformation("Getting all customer's complaint. PageNumber: {pageNumber}, PageSize: {pageSize}",
            request.PageNumber, PageSize); 
        
        var query = _dbContext.Complaints
            .Include(s=>s.Queue)
            .Where(s => s.CustomerId == customerId);
        
        if (!query.Any())
        {
            _logger.LogWarning("No complaint found for CustomerId: {customerId}", customerId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(ReviewEntity));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var complaints = await query
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Skip((request.PageNumber - 1) * 15)
            .Take(15).ToListAsync(cancellationToken);


        var response = complaints.Select(complaint => new ComplaintResponseModel()
        {
            Id = complaint.Id,
            CustomerId = complaint.CustomerId,
            QueueId = complaint.QueueId,
            EmployeeId = complaint.Queue.EmployeeId,
            ComplaintText = complaint.ComplaintText,
            ResponseText = complaint.ResponseText,
            ComplaintStatus = complaint.ComplaintStatus
        }).ToList();
        
                
        _logger.LogInformation("Successfully fetched {ComplaintCount} complaints for CustomerId: {CustomerId}",
            response.Count,
            customerId);
        
        
        _logger.LogInformation("Fetched {companyCount} companies.", response.Count);
        var pagedResponse=new PagedResponse<ComplaintResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = PageSize,
            TotalCount = totalCount
        };


        return pagedResponse;

    }
}