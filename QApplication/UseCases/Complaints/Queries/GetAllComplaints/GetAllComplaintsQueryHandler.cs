using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces.Data;
using QApplication.Responses;

namespace QApplication.UseCases.Complaints.Queries.GetAllComplaints;

public class GetAllComplaintsQueryHandler: IRequestHandler<GetAllComplaintsQuery, PagedResponse<ComplaintResponseModel>>
{
    private const int PageSize = 15;
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
            PageSize);

        var totalCount = await _dbContext.Complaints.CountAsync(cancellationToken);

        var dbComplaints =await  _dbContext.Complaints
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * PageSize)
            .Take(PageSize)
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
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}