using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Complaints.Queries.GetAllComplaints;

public class GetAllComplaintsQueryHandler: IRequestHandler<GetAllComplaintsQuery, PagedResponse<ComplaintResponseModel>>
{
    private readonly ILogger<GetAllComplaintsQueryHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public GetAllComplaintsQueryHandler(ILogger<GetAllComplaintsQueryHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<ComplaintResponseModel>> Handle(GetAllComplaintsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all complaints. PageNumber: {pageNumber}, PageSize: {pageSize}", request.PageNumber,
            request.PageSize);

        var totalCount = await _dbContext.Complaints.CountAsync(cancellationToken);

        var dbComplaints =await  _dbContext.Complaints
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = dbComplaints.Select(complaint => new ComplaintResponseModel()
        {
            Id = complaint.Id,
            CustomerId = complaint.CustomerId,
            QueueId = complaint.QueueId,
            ComplaintText = complaint.ComplaintText,
            ResponseText = complaint.ResponseText,
            ComplaintStatus = complaint.ComplaintStatus
        }).ToList();
        
        _logger.LogInformation("Fetched {complaintsCount} complaints.", response.Count);

        return new PagedResponse<ComplaintResponseModel>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}